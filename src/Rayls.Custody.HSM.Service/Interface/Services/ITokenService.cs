using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenByIdAsync(string id);
        Task<TokenResponse> GetTokenByAddressAsync(string address);
        Task<BalanceResponse> GetBalanceByTokenIdAsync(string id, string walletId);
        Task<(IEnumerable<TokenResponse>, int)> GetTokenListAsync(int pageNumber, int pageSize);
        Task<TokenResponse> RegisterTokenByAddress(string tokenAddress);
        Task<string> TransferTokenAsync(string tokenAddress, Account account, string to, HexBigInteger value);
        Task<string> TransferTokenBridgeAsync(BigInteger toChainId, string tokenAddress, string toTokenAddress, Nethereum.Web3.Accounts.Account account, string to, BigInteger value);
    }
}
