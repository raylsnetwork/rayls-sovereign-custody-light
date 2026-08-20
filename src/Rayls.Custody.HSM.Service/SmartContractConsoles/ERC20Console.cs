using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;
using Rayls.Custody.HSM.DTO.Configuration;
using Nethereum.Web3.Accounts;

// Generated via https://playground.nethereum.com/ using the ERC20 abi from https://gist.github.com/veox/8800debbf56e24718f9f483e1e40c35c
namespace Rayls.Custody.HSM.Service
{

    public class Erc20Console
    {
        private readonly ApiConfig _apiConfig;
        private Web3 _web3;

        public Erc20Console(ApiConfig apiConfig)
        {
            _apiConfig = apiConfig;
            // Web3 is built lazily (see Web3) so the console instantiates with no chain bound yet.
        }

        // Web3 is constructed on first use rather than in the constructor, so the console can be
        // resolved even when EVM_JSON_RPC is empty (before a chain is created). Only the read-only
        // contract-handler paths below (BalanceOf etc.) require it; the write paths build their own
        // signer-bound Web3 per call.
        private Web3 Web3 =>
            _web3 ??= string.IsNullOrWhiteSpace(_apiConfig.EVM_JSON_RPC)
                ? throw new CustodyHSMAPIValidationException("EVM_JSON_RPC is not configured — no chain is bound yet")
                : new Web3(_apiConfig.EVM_JSON_RPC);

        public async Task<string> Approve(string contractAddress, Account account, string spender, BigInteger value)
        {
            var web3 = new Web3(account, _apiConfig.EVM_JSON_RPC);
            var contractHandler = web3.Eth.GetContractHandler(contractAddress);
            var txReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(new ApproveFunction() { Spender = spender, Value = value });
            return txReceipt.TransactionHash;
        }

        public async Task<string> Transfer(string contractAddress, Account account, string to, BigInteger value)
        {
            var web3 = new Web3(account, _apiConfig.EVM_JSON_RPC);
            web3.TransactionManager.UseLegacyAsDefault = true;

            var contractHandler = web3.Eth.GetContractHandler(contractAddress);
            var transferFunction = new TransferFunction() { To = to, Value = value };
            transferFunction.Gas = new HexBigInteger("0xF4240");
            var txReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferFunction);
            return txReceipt.TransactionHash;
        }

        public async Task<string> TransferRequest(BigInteger toChainId, string contractAddress, Account account, string to, BigInteger value, string toTokenAddress)
        {
            var web3 = new Web3(account, _apiConfig.EVM_JSON_RPC);
            web3.TransactionManager.UseLegacyAsDefault = true;

            var contractHandler = web3.Eth.GetContractHandler(contractAddress);
            var transferRequestFunction = new TransferRequestFunction()
                {
                    ToChainId = toChainId,
                    DestinationToken = toTokenAddress,
                    To = to,
                    Amount = value
                };
            transferRequestFunction.Gas = new HexBigInteger("0xF4240");

            var txReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferRequestFunction);
            return txReceipt.TransactionHash;
        }

        public async Task<string> TransferFrom(string contractAddress, Account account, string from, string to, BigInteger value)
        {
            var web3 = new Web3(account, _apiConfig.EVM_JSON_RPC);
            var contractHandler = web3.Eth.GetContractHandler(contractAddress);
            var txReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(new TransferFromFunction() { From = from, To = to, Value = value });
            return txReceipt.TransactionHash;
        }


        public async Task<string> Name(string contractAddress)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<NameFunction, string>();
        }

        public async Task<string> Symbol(string contractAddress)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<SymbolFunction, string>();
        }

        public async Task<int> Decimals(string contractAddress)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<DecimalsFunction, int>();
        }

        public async Task<BigInteger> TotalSupply(string contractAddress)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<TotalSupplyFunction, BigInteger>();
        }

        public async Task<BigInteger> BalanceOf(string contractAddress, string owner)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<BalanceOfFunction, BigInteger>(new BalanceOfFunction() { Owner = owner });
        }

        public async Task<BigInteger> Allowance(string contractAddress, string owner, string spender)
        {
            var contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<AllowanceFunction, BigInteger>(new AllowanceFunction() { Owner = owner, Spender = spender });
        }

    }



    public partial class Erc20Deployment : Erc20DeploymentBase
    {
        public Erc20Deployment() : base(BYTECODE) { }
        public Erc20Deployment(string byteCode) : base(byteCode) { }
    }

    public class Erc20DeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public Erc20DeploymentBase() : base(BYTECODE) { }
        public Erc20DeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class NameFunction : NameFunctionBase { }

    [Function("name", "string")]
    public class NameFunctionBase : FunctionMessage
    {

    }

    public partial class ApproveFunction : ApproveFunctionBase { }

    [Function("approve", "bool")]
    public class ApproveFunctionBase : FunctionMessage
    {
        [Parameter("address", "_spender", 1)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "_value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TotalSupplyFunction : TotalSupplyFunctionBase { }

    [Function("totalSupply", "uint256")]
    public class TotalSupplyFunctionBase : FunctionMessage
    {

    }

    public partial class TransferFromFunction : TransferFromFunctionBase { }

    [Function("transferFrom", "bool")]
    public class TransferFromFunctionBase : FunctionMessage
    {
        [Parameter("address", "_from", 1)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 2)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 3)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class DecimalsFunction : DecimalsFunctionBase { }

    [Function("decimals", "uint8")]
    public class DecimalsFunctionBase : FunctionMessage
    {

    }

    public partial class BalanceOfFunction : BalanceOfFunctionBase { }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunctionBase : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public virtual string Owner { get; set; }
    }

    public partial class SymbolFunction : SymbolFunctionBase { }

    [Function("symbol", "string")]
    public class SymbolFunctionBase : FunctionMessage
    {

    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferRequestFunction : TransferRequestFunctionBase { }

    [Function("transferRequest", "bool")]
    public class TransferRequestFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_toChainId", 1)]
        public virtual BigInteger ToChainId { get; set; }

        [Parameter("address", "_destinationToken", 2)]
        public virtual string DestinationToken { get; set; }

        [Parameter("address", "_to", 3)]
        public virtual string To { get; set; }

        [Parameter("uint256", "_amount", 4)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class AllowanceFunction : AllowanceFunctionBase { }

    [Function("allowance", "uint256")]
    public class AllowanceFunctionBase : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("address", "_spender", 2)]
        public virtual string Spender { get; set; }
    }

    public partial class ApprovalEventDTO : ApprovalEventDTOBase { }

    [Event("Approval")]
    public class ApprovalEventDTOBase : IEventDTO
    {
        [Parameter("address", "owner", 1, true)]
        public virtual string Owner { get; set; }
        [Parameter("address", "spender", 2, true)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "value", 3, false)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase { }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2, true)]
        public virtual string To { get; set; }
        [Parameter("uint256", "value", 3, false)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class NameOutputDTO : NameOutputDTOBase { }

    [FunctionOutput]
    public class NameOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }



    public partial class TotalSupplyOutputDTO : TotalSupplyOutputDTOBase { }

    [FunctionOutput]
    public class TotalSupplyOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class DecimalsOutputDTO : DecimalsOutputDTOBase { }

    [FunctionOutput]
    public class DecimalsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint8", "", 1)]
        public virtual byte ReturnValue1 { get; set; }
    }

    public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }

    [FunctionOutput]
    public class BalanceOfOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "balance", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class SymbolOutputDTO : SymbolOutputDTOBase { }

    [FunctionOutput]
    public class SymbolOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }



    public partial class AllowanceOutputDTO : AllowanceOutputDTOBase { }

    [FunctionOutput]
    public class AllowanceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }
}




/** Function: transferFrom**/
/*
var transferFromFunction = new TransferFromFunction();
transferFromFunction.From = from;
transferFromFunction.To = to;
transferFromFunction.Value = value;
var transferFromFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction);
*/

/** Function: transfer**/
/*
var transferFunction = new TransferFunction();
transferFunction.To = to;
transferFunction.Value = value;
var transferFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferFunction);
*/
