using Rayls.Custody.HSM.Repository.PostgreSql;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System.Linq;
using System;

namespace Rayls.Custody.HSM.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly RaylzDbContext _raylzDbContext;

        public TransactionRepository(RaylzDbContext raylzDbContext)
        {
            _raylzDbContext = raylzDbContext;
        }

        public async Task<Transaction> GetByIdAsync(string transactionId)
        {
            return _raylzDbContext.Transactions.FirstOrDefault(x => x.Id == transactionId);
        }

        public async Task<(IEnumerable<Transaction>, int)> GetListAsync(int pageNumber, int pageSize)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;

            var paginatedItems = _raylzDbContext.Transactions.Skip(itemsToSkip).Take(pageSize).ToList();

            int totalRecords = _raylzDbContext.Transactions.Count();

            return (paginatedItems, totalRecords);
        }

        public async Task SaveAsync(Transaction model)
        {
            model.Id = Guid.NewGuid().ToString();
            _raylzDbContext.Transactions.Add(model);
            _raylzDbContext.SaveChanges();
        }
    }
}
