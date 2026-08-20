using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Services
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletResponse>> CreateWalletAsync(WalletRequest request);
        Task<WalletResponse> GetWalletByIdAsync(string id);
        Task<WalletResponse> GetWalletByAddressAsync(string address);
        Task<(IEnumerable<WalletResponse>, int)> GetWalletListAsync(int pageNumber, int pageSize);
        // chainId is the target chain the returned account signs for (EIP-155). Pass null to fall
        // back to the process-wide EVM_JSON_RPC_CHAIN_ID.
        Task<Nethereum.Web3.Accounts.Account> GetAwsKmsAccount(Wallet wallet, int index, string chainId = null);
        Task<Nethereum.Web3.Accounts.Account> GetKeyStoreAccount(Wallet wallet, string password, string chainId = null);
        Task<Wallet> GetWalletModel(string id);
        Task AssociateWalletExternalIdAsync(string id, string externalId);
    }
}
