using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.DTO.ClientProviders
{
    public class CustodyHSMAPIClientProvider : CustodyHSMClientProvider
    {
        private string _apiUrl;

        public CustodyHSMAPIClientProvider()
        {
        }

        public void setApiUrl(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public CustodyHSMAPIRequest GetRequest(Method method, string path, string token = null, int timeout = 300000, string contentType = "application/json")
        {
            return new CustodyHSMAPIRequest(_apiUrl).Create(method, path, token, timeout, contentType);
        }

        public override void BeforeExecute()
        {
            BaseUrl = new Uri(_apiUrl);
        }

        public override Task<IRestResponse<T>> RequestAsync<T, L>(IRestRequest request, IHttpContextAccessor context) where L : class
        {
            this.BeforeExecute();

            return base.RequestAsync<T, L>(request, context);
        }

        public override CustodyHSMClientProvider IsAuthAction()
        {
            return this;
        }
    }
}
