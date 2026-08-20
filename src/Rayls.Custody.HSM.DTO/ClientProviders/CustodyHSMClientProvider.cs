using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rayls.Custody.HSM.DTO.ClientProviders.Interfaces;
using RestSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.DTO.ClientProviders
{
    public abstract class CustodyHSMClientProvider : RestClient, IRestClient, ICustodyHSMClientProvider
    {
        protected ILogger _logger;

        public abstract CustodyHSMClientProvider IsAuthAction();

        public CustodyHSMClientProvider()
        {
        }

        public virtual void BeforeExecute()
        {
        }

        public virtual async Task<IRestResponse<T>> RequestAsync<T, L>(IRestRequest request, IHttpContextAccessor context) where L : class
        {
            string key = "x-tracekey";
            var keyInHeader = context?.HttpContext?.Request?.Headers?
                .FirstOrDefault(x =>
                x.Key.ToLower().Equals(key) ||
                x.Key.ToLower().Equals(key.Replace("x-", "")));

            string tracekey = "";

            if (keyInHeader.HasValue && keyInHeader.Value.Value.ToString() != string.Empty)
            {
                tracekey = keyInHeader.Value.Value;
                request.AddHeader(key, tracekey);
            }
            else
            {
                tracekey = string.Format("internal-{0}", Guid.NewGuid().ToString());
                request.AddHeader(key, tracekey);
            }

            IRestResponse<T> response = new RestResponse<T>();

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                response = await ExecuteAsync<T>(request, new CancellationTokenSource().Token);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in ExecuteAsync");
                throw;
            }
            finally
            {
                stopWatch.Stop();

                _logger?.LogInformation("ClientProvider: TraceKey={TraceKey} Url={Url} Status={Status} ElapsedMs={ElapsedMs}",
                    tracekey, request.Resource, (int)response.StatusCode, stopWatch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}
