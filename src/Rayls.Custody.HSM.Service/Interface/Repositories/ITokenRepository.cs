using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Repositories
{
    public interface ITokenRepository
    {
        Task<Token> GetByIdAsync(string id);
        Task<Token> GetByAddressAsync(string address);
        Task<(IEnumerable<Token>, int)> GetListAsync(int pageNumber, int pageSize);
        Task SaveAsync(Token model);
    }
}
