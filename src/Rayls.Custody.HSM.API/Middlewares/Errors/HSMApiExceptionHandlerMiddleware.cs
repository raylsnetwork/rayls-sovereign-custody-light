using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Errors;
using Rayls.Custody.HSM.Service;
using System;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.API.Middlewares
{
    public class HSMApiExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HSMApiExceptionHandlerMiddleware> _logger;

        public HSMApiExceptionHandlerMiddleware(RequestDelegate next, ILogger<HSMApiExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                await HandleException(httpContext, exception);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var custodyApiErrorDTO = new HSMApiErrorDTO(timestamp: DateTime.UtcNow, traceKey: context.TraceIdentifier);

            switch (ex)
            {
                case CustodyHSMAPIValidationException hSMAPIValidationException:
                    {
                        context.Response.StatusCode = hSMAPIValidationException.StatusCode;
                        custodyApiErrorDTO.AddError(new HSMErrorDTO("HSM-001", hSMAPIValidationException.Message));
                        LogException(ex, context, hSMAPIValidationException.Message, hSMAPIValidationException.DetailedMessage);
                        break;
                    }
                default:
                    {
                        context.Response.StatusCode = 500;
                        custodyApiErrorDTO.AddError(errorCode: "HSM-002", errorMessage: "An internal server error occurred.");
                        LogException(ex, context, string.Empty, string.Empty);
                        break;
                    }
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(custodyApiErrorDTO, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        private void LogException(Exception exception, HttpContext context, string errorMessage, string detailMessage)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}. Error: {ErrorMessage}. Detail: {DetailMessage}",
                context.TraceIdentifier, errorMessage, detailMessage);
        }

    }
}
