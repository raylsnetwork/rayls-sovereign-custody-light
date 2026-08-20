using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Const;
using Rayls.Custody.HSM.Service.Interface.Services;
using Rayls.Custody.HSM.DTO.Errors;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <param name="request"></param>
        [SwaggerOperation(Summary = "Create Token", Tags = new string[] { "Token" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TokenResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.CreateToken)]
        [Authorize]
        public async Task<IActionResult> RegisterTokenByAddress(
            [FromBody] TokenRequest request)
        {

            TokenResponse response = await _tokenService.RegisterTokenByAddress(request.Address);
            return Ok(response);

        }

        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        [SwaggerOperation(Summary = "Get Token List", Tags = new string[] { "Token" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof((IEnumerable<TokenResponse>, int)))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetTokenList)]
        [Authorize]
        public async Task<IActionResult> GetTokenListAsync(
            [FromQuery(Name = "page_number")] int pageNumber = 1,
            [FromQuery(Name = "page_size")] int pageSize = 100)
        {

            var response = await _tokenService.GetTokenListAsync(pageNumber, pageSize);
            return Ok(response);

        }

        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        [SwaggerOperation(Summary = "Get Balance by id", Tags = new string[] { "Token" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof((IEnumerable<TokenResponse>, int)))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetBalanceByTokenId)]
        [Authorize]
        public async Task<IActionResult> GetBalanceByIdAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "walletId")] string walletId)
        {

            var response = await _tokenService.GetBalanceByTokenIdAsync(id, walletId);
            return Ok(response);

        }
        /// <param name="id"></param>
        [SwaggerOperation(Summary = "Get Token By Id", Tags = new string[] { "Token" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TokenResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetTokenById)]
        [Authorize]
        public async Task<IActionResult> GetTokenByIdAsync(
            [FromRoute(Name = "id")] string id)
        {

            TokenResponse response = await _tokenService.GetTokenByIdAsync(id);
            return Ok(response);

        }

    }
}
