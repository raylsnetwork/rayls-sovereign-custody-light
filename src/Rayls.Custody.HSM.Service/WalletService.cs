using AutoMapper;
using NBitcoin;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore.Model;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Configuration;
using Rayls.Custody.HSM.DTO.Enums;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using Rayls.Custody.HSM.Service.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly ApiConfig _apiConfig;
        private readonly IHsm _hsmClient;
        private Web3 _web3;

        public WalletService(
            IHsm hsmClient,
            IWalletRepository walletRepository,
            IMapper mapper,
            ApiConfig apiConfig)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
            _hsmClient = hsmClient;
            _apiConfig = apiConfig;
            // Web3 is built lazily (see Web3) so wallet creation works with no chain bound yet.
        }

        // Web3 is constructed on first use rather than in the constructor: address/keystore
        // generation needs no chain, so the service must instantiate even when EVM_JSON_RPC is
        // empty (before a chain is created). Only on-chain reads (GetAddressBalance) require it.
        private Web3 Web3 =>
            _web3 ??= string.IsNullOrWhiteSpace(_apiConfig.EVM_JSON_RPC)
                ? throw new CustodyHSMAPIValidationException("EVM_JSON_RPC is not configured — no chain is bound yet")
                : new Web3(_apiConfig.EVM_JSON_RPC);

        public async Task<IEnumerable<WalletResponse>> CreateWalletAsync(WalletRequest request)
        {
            if (request == null)
                throw new CustodyHSMAPIValidationException("Invalid request");

            if (request.AddressQuantity == null)
                throw new CustodyHSMAPIValidationException("AddressQuantity Required");

            if (request.AddressQuantity == 0)
                throw new CustodyHSMAPIValidationException("Must create at least 1 wallet");             


            List<WalletResponse> walletResponseList = new List<WalletResponse>();

            switch (request.Type)
            {
                case WalletType.KEYSTORE_V3:
                    {
                        if (String.IsNullOrEmpty(request.Password))
                            throw new CustodyHSMAPIValidationException("Password Required to KEYSTORE_V3 wallets");

                        if (request.AddressQuantity > int.Parse(_apiConfig.KEYSTORE_MAX_WALLETS))
                            throw new CustodyHSMAPIValidationException($"For KEYSTORE_V3 the max quantity for generation is {_apiConfig.KEYSTORE_MAX_WALLETS}");

                        var walletsToSave = new List<Wallet>();
                        for (int i = 1; i <= request.AddressQuantity.Value; i++)
                        {
                            var wallet = CreateKeyStore(request.Password);
                            walletsToSave.Add(wallet);
                        }
                        await _walletRepository.SaveBulkAsync(walletsToSave);
                        walletResponseList = walletsToSave.Select(wallet => _mapper.Map<WalletResponse>(wallet)).ToList();
                        break;
                    }
                case WalletType.AWS_KMS:
                    {

                        if (request.AddressQuantity > int.Parse(_apiConfig.KMS_MAX_WALLETS))
                            throw new CustodyHSMAPIValidationException($"For KMS the max quantity for generation is {_apiConfig.KMS_MAX_WALLETS}");

                        var walletsToSave = await CreateHDWallet(request.AddressQuantity);
                        await _walletRepository.SaveBulkAsync(walletsToSave);
                        walletResponseList = walletsToSave.Select(wallet => _mapper.Map<WalletResponse>(wallet)).ToList();
                        break;
                    }
            }


            return walletResponseList;
        }

        private static Wallet CreateKeyStore(string password)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();

            var scryptParams = new ScryptParams { Dklen = 32, N = 262144, R = 1, P = 8 };
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            var json = keyStoreService.SerializeKeyStoreToJson(keyStore);

            Wallet wallet = new()
            {
                Data = json,
                Type = WalletType.KEYSTORE_V3,
                Address = keyStore.Address,
                PublicKey = ecKey.GetPubKey().ToHex()
            };
            return wallet;
        }

        public async Task<(IEnumerable<WalletResponse>, int)> GetWalletListAsync(int pageNumber, int pageSize)
        {
            var list = await _walletRepository.GetListAsync(pageNumber, pageSize);

            var response = _mapper.Map<IEnumerable<WalletResponse>>(list.Item1);

            return (response, list.Item2);
        }

        public async Task<WalletResponse> GetWalletByIdAsync(string id)
        {
            var model = await _walletRepository.GetByIdAsync(id);
            var response = _mapper.Map<WalletResponse>(model);
            return response;
        }

        public async Task<WalletResponse> GetWalletByAddressAsync(string address)
        {
            var model = await _walletRepository.GetByAddressAsync(address);
            var response = _mapper.Map<WalletResponse>(model);
            return response;
        }

        public async Task<Wallet> GetWalletModel(string id) => await _walletRepository.GetByIdAsync(id);


        public async Task<List<Wallet>> CreateHDWallet(int? qtdAddress = 1)
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var hdWallet = new Nethereum.HdWallet.Wallet(mnemo.ToString(), null);

            var keyId = await GetOrCreateKMSKey();
            var mnemoEncrypted = await _hsmClient.Encrypt(keyId, mnemo.ToString());

            List<Wallet> walletsResponse = new();

            for (int i = (int)qtdAddress - 1; i >= 0; i--)
            {
                var account = hdWallet.GetAccount(i);
                Wallet wallet = new()
                {
                    Data = mnemoEncrypted,
                    Type = WalletType.AWS_KMS,
                    Address = account.Address,
                    PublicKey = account.PublicKey,
                    Index = i,
                    KMSKeyId = keyId
                };
                walletsResponse.Add(wallet);
            }
            return walletsResponse;
        }

        private async Task<string> GetOrCreateKMSKey()
        {
            var keyExist = await _hsmClient.FindKey();
            String keyId = String.Empty;
            if (keyExist != null)
            {
                keyId = keyExist;
            }
            else
            {
                keyId = await _hsmClient.CreateKey();
            }
            return keyId;
        }

        // Resolves the chain ID an account signs for: the per-request value when the caller supplied
        // one, otherwise the process-wide default. The default is empty whenever the service booted
        // before any chain existed, so reject that explicitly — BigInteger.Parse("") would otherwise
        // surface as an opaque "The value could not be parsed" FormatException from deep in the stack.
        private BigInteger ResolveChainId(string chainId)
        {
            var raw = string.IsNullOrWhiteSpace(chainId) ? _apiConfig.EVM_JSON_RPC_CHAIN_ID : chainId;

            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new CustodyHSMAPIValidationException(
                    "No chain ID for this request: pass ChainId, or configure EVM_JSON_RPC_CHAIN_ID.");
            }

            if (!BigInteger.TryParse(raw, out var parsed))
            {
                throw new CustodyHSMAPIValidationException($"Chain ID '{raw}' is not a valid integer.");
            }

            return parsed;
        }

        public async Task<Nethereum.Web3.Accounts.Account> GetAwsKmsAccount(Wallet wallet, int index, string chainId = null)
        {
            var keyId = await GetOrCreateKMSKey();
            var mnemoDecrypted = await _hsmClient.Decrypt(keyId, wallet.Data);
            return (new Nethereum.HdWallet.Wallet(mnemoDecrypted, null)).GetAccount(index, ResolveChainId(chainId));
        }

        public async Task<Account> GetKeyStoreAccount(Wallet wallet, string password, string chainId = null)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, wallet.Data);

            var account = new Account(key, ResolveChainId(chainId));
            return account;
        }

        public async Task AssociateWalletExternalIdAsync(string id, string externalId)
        {
            await _walletRepository.AssociateWalletExternalIdAsync(id, externalId);
        }

        public async Task<Nethereum.Hex.HexTypes.HexBigInteger> GetAddressBalance(string address)
        {
            return await Web3.Eth.GetBalance.SendRequestAsync(address);
        }
    }
}
