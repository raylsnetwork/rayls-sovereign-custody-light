using Rayls.Custody.HSM.Service.Interface.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Repositories
{
      public interface IHsm
    {
        Task TestConnection();
        Task<string> FindKey();
        Task<string> CreateKey();
        Task<string> Encrypt(string keyId, string plaintext);
        Task<string> Decrypt(string keyId, string base64Ciphertext);
    }
}
