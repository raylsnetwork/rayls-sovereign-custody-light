using Rayls.Custody.HSM.DTO.ClientProviders.Interfaces;
using RestSharp;

namespace Rayls.Custody.HSM.DTO.ClientProviders
{
    public class CustodyHSMAPIRequest : RestRequest, ICustodyHSMClientProviderRequest
    {
        private readonly string _apiUrl;

        public CustodyHSMAPIRequest(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public CustodyHSMAPIRequest Create(Method method, string path, string token = null, int timeout = 300000, string contentType = "application/json")
        {
            base.Method = method;
            base.Resource = $"{_apiUrl}{path}";
            base.RequestFormat = DataFormat.Json;
            base.Timeout = timeout;
            base.AddHeader("Content-Type", contentType);

            if (!string.IsNullOrEmpty(token))
            {
                base.AddHeader("Authorization", string.Format("Bearer {0}", token));
            }

            return this;
        }
    }
}
