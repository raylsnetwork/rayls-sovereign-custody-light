using Rayls.Custody.HSM.Repository.PostgreSql;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly RaylzDbContext _raylzDbContext;

        public WalletRepository(RaylzDbContext raylzDbContext)
        {
            _raylzDbContext = raylzDbContext;
        }

        public async Task<Wallet> GetByIdAsync(string walletId)
        {
            return _raylzDbContext.Wallets.FirstOrDefault(x => x.Id == walletId);
        }

        public async Task<Wallet> GetByAddressAsync(string address)
        {
            return _raylzDbContext.Wallets.FirstOrDefault(x => x.Address == address);
        }

        public async Task<(IEnumerable<Wallet>, int)> GetListAsync(int pageNumber, int pageSize)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;

            var paginatedItems = _raylzDbContext.Wallets.Skip(itemsToSkip).Take(pageSize).ToList();

            int totalRecords = _raylzDbContext.Wallets.Count();

            return (paginatedItems, totalRecords);
        }

        public async Task SaveAsync(Wallet model)
        {
            _raylzDbContext.Wallets.Add(model);
            _raylzDbContext.SaveChanges();
        }

        public async Task SaveBulkAsync(IEnumerable<Wallet> models)
        {
            _raylzDbContext.Wallets.AddRange(models);
            _raylzDbContext.SaveChanges(); 
        }

        public async Task AssociateWalletExternalIdAsync(string id, string externalId)
        {
            var model = await GetByIdAsync(id);
            model.ExternalId = externalId;

            _raylzDbContext.Wallets.Update(model);
            _raylzDbContext.SaveChanges();
        }
    }
}
