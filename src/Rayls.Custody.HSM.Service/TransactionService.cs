using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Configuration;
using Rayls.Custody.HSM.DTO.Enums;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Services;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Transaction = Rayls.Custody.HSM.Service.Interface.Repositories.Model.Transaction;

namespace Rayls.Custody.HSM.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletService _walletService;
        private readonly ApiConfig _apiConfig;
        private Web3 _web3;
        private readonly ITokenService _tokenService;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IWalletService walletService,
            ApiConfig apiConfig,
            ITokenService tokenService)
        {
            _transactionRepository = transactionRepository;
            _walletService = walletService;
            _apiConfig = apiConfig;
            // Web3 is built lazily (see Web3) so the service instantiates with no chain bound yet.
            _tokenService = tokenService;
        }

        // Web3 is constructed on first use rather than in the constructor, so the service can be
        // resolved even when EVM_JSON_RPC is empty (before a chain is created). Only the on-chain
        // read paths below (GetTransactionByHash/Receipt, ViewTransaction) require it.
        private Web3 Web3 =>
            _web3 ??= new Web3(ResolveRpcUrl(null));

        // Resolves the JSON-RPC endpoint for a request: the per-request URL when the caller supplied
        // one, otherwise the process-wide default. The default is empty when the service booted
        // before any chain existed, so say that plainly rather than letting Web3 fail on an empty URI.
        private string ResolveRpcUrl(string rpcUrl)
        {
            if (!string.IsNullOrWhiteSpace(rpcUrl)) return rpcUrl;

            if (string.IsNullOrWhiteSpace(_apiConfig.EVM_JSON_RPC))
            {
                throw new CustodyHSMAPIValidationException(
                    "No JSON-RPC URL for this request: pass RpcUrl, or configure EVM_JSON_RPC.");
            }

            return _apiConfig.EVM_JSON_RPC;
        }

        public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransactionEvmBase> request)
        {
            TransactionResponse response = new TransactionResponse();
            Account account = await GetAccountAsync(request.WalletId, request.Password, request.ChainId);

            var web3 = new Web3(account, ResolveRpcUrl(request.RpcUrl));
            var transaction = new Nethereum.RPC.Eth.DTOs.TransactionInput()
            {
                To = request.Transaction.To,
                Data = request.Transaction.Data,
                From = account.Address,
                Value = new HexBigInteger(BigInteger.Parse(request.Transaction.Value ?? "0")),
            };

            if (!string.IsNullOrEmpty(request.Transaction.Nonce))
            {
                transaction.Nonce = new HexBigInteger(BigInteger.Parse(request.Transaction.Nonce));
            }

            web3.TransactionManager.UseLegacyAsDefault = true;
            // `To` must be propagated to estimateGas: when it's null the RPC node
            // interprets the call as a contract deploy and executes `Data` as
            // init-code, which makes regular method-call payloads fail with
            // OpcodeNotFound on the first selector byte.
            var estimateGas = await web3.Eth.TransactionManager.EstimateGasAsync(new CallInput()
            {
                To = transaction.To,
                Data = transaction.Data,
                From = transaction.From,
                Value = transaction.Value
            });

            // Apply a 50% safety buffer to the estimate. eth_estimateGas systematically
            // under-estimates for calls that fan out into delegatecalls / cross-chain
            // dispatch, because the binary search doesn't reserve the
            // EIP-150 63/64 headroom the inner calls need. A raw estimate reverts on-chain
            // (observed: estimate 91251 vs 94073 actually required). Unused gas is refunded,
            // so over-allocating is harmless for these small (~100k) contract calls.
            transaction.Gas = new HexBigInteger(estimateGas.Value * 3 / 2);
            var sendTransaction = await web3.Eth.TransactionManager.SendTransactionAndWaitForReceiptAsync(transaction);
            response.TxHash = sendTransaction?.TransactionHash;

            var transactionModel = new Transaction
            {
                OperationType = OperationType.TRANSACTION,
                From = account.Address,
                TxHash = response.TxHash
            };

            await _transactionRepository.SaveAsync(transactionModel);

            return response;
        }

        public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransferTokenBase> request)
        {
            TransactionResponse response = new TransactionResponse();
            Account account = await GetAccountAsync(request.WalletId, request.Password, request.ChainId);

            TokenResponse token = await _tokenService.GetTokenByIdAsync(request.Transaction.TokenId);

            response.TxHash = await _tokenService.TransferTokenAsync(token.Address, account, request.Transaction.To, new HexBigInteger(BigInteger.Parse(request.Transaction.Value)));

            var transactionModel = new Transaction
            {
                OperationType = OperationType.TRANSFER_TOKEN,
                From = account.Address,
                TxHash = response.TxHash
            };

            await _transactionRepository.SaveAsync(transactionModel);

            return response;
        }

        public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransferTokenBridgeBase> request)
        {
            TransactionResponse response = new TransactionResponse();
            Account account = await GetAccountAsync(request.WalletId, request.Password, request.ChainId);

            TokenResponse token = await _tokenService.GetTokenByIdAsync(request.Transaction.TokenId);

            response.TxHash = await _tokenService.TransferTokenBridgeAsync(BigInteger.Parse(request.Transaction.ToChainId), token.Address, request.Transaction.ToTokenAddress, account, request.Transaction.To, BigInteger.Parse(request.Transaction.Value));

            var transactionModel = new Transaction
            {
                OperationType = OperationType.TRANSFER_TOKEN_BRIDGE,
                From = account.Address,
                TxHash = response.TxHash
            };

            await _transactionRepository.SaveAsync(transactionModel);

            return response;
        }

        // chainId names the chain the returned account signs for; null falls back to the configured default.
        private async Task<Account> GetAccountAsync(string walletId, string password = null, string chainId = null)
        {
            Account account = null;
            var wallet = await _walletService.GetWalletModel(walletId) ?? throw new CustodyHSMAPIValidationException("Wallet not found.");

            switch (wallet.Type)
            {
                case WalletType.KEYSTORE_V3:
                    {
                        if (String.IsNullOrEmpty(password)) throw new CustodyHSMAPIValidationException("Password Required to KEYSTORE_V3 wallets");
                        account = await _walletService.GetKeyStoreAccount(wallet, password, chainId);
                        break;
                    }
                case WalletType.AWS_KMS:
                    {
                        account = await _walletService.GetAwsKmsAccount(wallet, (int)wallet.Index, chainId);
                        break;
                    }

            }

            return account;
        }

        public async Task<Nethereum.RPC.Eth.DTOs.Transaction> GetTransactionByHash(string hash)
        {
            return await Web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(hash);
        }

        public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> GetTransactionReceiptByHash(string hash)
        {
            return await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hash);
        }

        public async Task<ViewTransactionResponse> ViewTransactionAsync(ViewTransactionRequest request)
        {
            CallInput input = new CallInput();
            input.From = request.FromAddress;
            input.To = request.ContractAddress;
            input.Data = request.Data;
            var data = await Web3.Eth.Transactions.Call.SendRequestAsync(input);
            return new ViewTransactionResponse()
            {
                Data = data
            };
        }
    }
}
