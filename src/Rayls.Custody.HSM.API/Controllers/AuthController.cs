using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Const;
using Rayls.Custody.HSM.Service.Interface.Services;
using Rayls.Custody.HSM.DTO.Errors;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <param name="request"></param>
        [SwaggerOperation(Summary = "Authorize", Tags = new string[] { "Auth" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TokenResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.Auth)]
        public async Task<IActionResult> Authorize(
            [FromBody] AuthRequest request)
        {
            try
            {
                var response = await _authService.GenerateBearerToken(request.ApiKey);
                return Ok(new AuthResponse() { Token = response });
            }
            catch (UnauthorizedApiKeyException)
            {
                return Unauthorized("Invalid apiKey");
            }
            catch (Exception ex)
            {
                // Anything else is a server-side fault (e.g. a misconfigured
                // AUTH_JWT_SECRET). Reporting it as "Invalid apiKey" sends callers
                // hunting for a credential problem that does not exist.
                _logger.LogError(ex, "Failed to generate a bearer token");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate a bearer token");
            }
        }


    }
}
