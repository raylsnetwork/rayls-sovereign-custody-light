using Microsoft.AspNetCore.Builder;

namespace Rayls.Custody.HSM.API.Middlewares
{
    public static class HSMApiExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseHsmApiExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<HSMApiExceptionHandlerMiddleware>();
        }
    }
}
