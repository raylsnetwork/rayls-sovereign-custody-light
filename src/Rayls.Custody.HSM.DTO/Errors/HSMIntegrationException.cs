using System;

namespace Rayls.Custody.HSM.DTO.Errors
{
    public class HSMIntegrationException : Exception
    {
        public string ApiName { get; }
        public string Resource { get; }
        public int StatusCode { get; }

        public HSMIntegrationException(string message, string apiName, string resource, int statusCode)
            : base(message)
        {
            ApiName = apiName;
            Resource = resource;
            StatusCode = statusCode;
        }
    }
}
