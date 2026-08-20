using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Services
{
    public interface IAuthService
    {
        Task<string> GenerateBearerToken(string apiKey);
    }
}
