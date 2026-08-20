using Rayls.Custody.HSM.Repository.PostgreSql;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly RaylzDbContext _raylzDbContext;

        public TokenRepository(RaylzDbContext raylzDbContext)
        {
            _raylzDbContext = raylzDbContext;
        }

        public async Task<Token> GetByIdAsync(string tokenId)
        {
            return _raylzDbContext.Tokens.FirstOrDefault(x => x.Id == tokenId);
        }

        public async Task<Token> GetByAddressAsync(string address)
        {
            return _raylzDbContext.Tokens.FirstOrDefault(x => x.Address.ToLower() == address.ToLower());
        }

        public async Task<(IEnumerable<Token>, int)> GetListAsync(int pageNumber, int pageSize)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;

            var paginatedItems = _raylzDbContext.Tokens.Skip(itemsToSkip).Take(pageSize).ToList();

            int totalRecords = _raylzDbContext.Tokens.Count();

            return (paginatedItems, totalRecords);
        }

        public async Task SaveAsync(Token model)
        {
            model.Id = Guid.NewGuid().ToString();
            _raylzDbContext.Tokens.Add(model);
            _raylzDbContext.SaveChanges();
        }
    }
}
