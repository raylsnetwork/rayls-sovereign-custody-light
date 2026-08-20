using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rayls.Custody.HSM.API
{
    /// <summary>
    /// Startup Http
    /// </summary>
    public partial class Startup
    {
        private void ConfigureHttp(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
