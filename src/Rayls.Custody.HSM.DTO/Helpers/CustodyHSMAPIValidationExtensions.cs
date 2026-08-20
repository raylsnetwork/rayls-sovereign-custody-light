using Newtonsoft.Json.Linq;
using Rayls.Custody.HSM.DTO.Errors;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rayls.Custody.HSM.DTO.Helpers
{
    public static class CustodyHSMAPIValidationExtensions
    {
        public static void HandleCustodyHSMResponse<T>(this IRestResponse<T> response, string apiName)
        {
            if ((int)response.StatusCode >= 400 || response.IsSuccessful == false)
            {
                var errorMessage = response.ErrorMessage;
                errorMessage += GetCustodyHSMError(response.Content);

                var custodyHSMException = new HSMIntegrationException(errorMessage, apiName, response.Request.Resource,
                    (int)response.StatusCode);

                throw custodyHSMException;
            }
        }

        private static string GetCustodyHSMError(string content)
        {
            if (IsJson(content))
            {
                IList<JToken> obj = JObject.Parse(content);
                if (((JProperty)obj[0]).FirstOrDefault().Count() == 0)
                {

                    return ((JProperty)obj[0]).FirstOrDefault().ToString();
                }
            }

            return content;
        }

        private static bool IsJson(string str)
        {
            try
            {
                JObject.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
