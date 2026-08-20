using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Microsoft.Extensions.Caching.Memory;
using Rayls.Custody.HSM.DTO.Configuration;
using Rayls.Custody.HSM.Service.Interface.Repositories;

namespace Rayls.Custody.HSM.Repository
{
    public class AwsKmsHelper : IHsm
    {
        private readonly IMemoryCache _cache;
        private readonly AmazonKeyManagementServiceClient _kmsClient;
        public static string KeyId = "CustodyLight";


        public AwsKmsHelper(IMemoryCache cache, ApiConfig apiConfig)
        {
            _cache = cache;

            if (apiConfig.USES_AWS)
                _kmsClient = new AmazonKeyManagementServiceClient();
        }

        public async Task TestConnection()
        {
            await _kmsClient.ListAliasesAsync(new ListAliasesRequest());
        }

        public async Task<string> FindKey()
        {
            string keyIdFromCache;
            if(_cache.TryGetValue<string>(KeyId, out keyIdFromCache))
                return keyIdFromCache;
            
            var keyAlias = KeyId;
            var listKeys = await _kmsClient.ListAliasesAsync(new ListAliasesRequest());
            if (listKeys == null)
                return null;

            foreach (var key in listKeys.Aliases)
            {
                if (key.AliasName == $"alias/{keyAlias}"){
                    _cache.Set<string>(KeyId, key.TargetKeyId);
                    return key.TargetKeyId;
                }
            }

            return null;
        }
        public async Task<string> CreateKey()
        {
            var keyAlias = KeyId;
            var createKeyResponse = await _kmsClient.CreateKeyAsync(new CreateKeyRequest());
            var keyId = createKeyResponse.KeyMetadata.KeyId;


            await _kmsClient.CreateAliasAsync(new CreateAliasRequest
            {
                AliasName = $"alias/{keyAlias}",
                TargetKeyId = keyId
            });

            return keyId;
        }

        public async Task<string> Encrypt(string keyId, string plaintext)
        {
            var encryptRequest = new EncryptRequest
            {
                KeyId = keyId,
                Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plaintext))
            };

            var encryptResponse = await _kmsClient.EncryptAsync(encryptRequest);
            return Convert.ToBase64String(encryptResponse.CiphertextBlob.ToArray());
        }

        public async Task<string> Decrypt(string keyId, string base64Ciphertext)
        {
            var ciphertextBytes = Convert.FromBase64String(base64Ciphertext);
            var decryptRequest = new DecryptRequest
            {
                KeyId = keyId,
                CiphertextBlob = new MemoryStream(ciphertextBytes)
            };

            var decryptResponse = await _kmsClient.DecryptAsync(decryptRequest);
            return Encoding.UTF8.GetString(decryptResponse.Plaintext.ToArray());
        }

    }
}