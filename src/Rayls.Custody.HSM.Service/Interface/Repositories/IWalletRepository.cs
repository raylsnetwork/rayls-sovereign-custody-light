using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> GetByIdAsync(string id);
        Task<Wallet> GetByAddressAsync(string address);
        Task<(IEnumerable<Wallet>, int)> GetListAsync(int pageNumber, int pageSize);
        Task SaveAsync(Wallet model);
        Task SaveBulkAsync(IEnumerable<Wallet> models);
        Task AssociateWalletExternalIdAsync(string id, string externalId);
    }
}
