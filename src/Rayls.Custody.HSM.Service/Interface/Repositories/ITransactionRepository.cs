using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(string id);
        Task<(IEnumerable<Transaction>, int)> GetListAsync(int pageNumber, int pageSize);
        Task SaveAsync(Transaction model);
    }
}
