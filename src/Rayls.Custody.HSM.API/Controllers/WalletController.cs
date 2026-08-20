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
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <param name="request"></param>
        [SwaggerOperation(Summary = "Create Wallet", Tags = new string[] { "Wallet" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(IEnumerable<WalletResponse>))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.CreateWallet)]
        [Authorize]
        public async Task<IActionResult> CreateWalletAsync(
            [FromBody] WalletRequest request)
        {

            IEnumerable<WalletResponse> response = await _walletService.CreateWalletAsync(request);
            return Ok(response);

        }

        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        [SwaggerOperation(Summary = "Get Wallet List", Tags = new string[] { "Wallet" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof((IEnumerable<WalletResponse>, int)))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetWalletList)]
        [Authorize]
        public async Task<IActionResult> GetWalletListAsync(
            [FromQuery(Name = "page_number")] int pageNumber = 1,
            [FromQuery(Name = "page_size")] int pageSize = 100)
        {

            var response = await _walletService.GetWalletListAsync(pageNumber, pageSize);
            return Ok(response);

        }

        /// <param name="id"></param>
        [SwaggerOperation(Summary = "Get Wallet By Id", Tags = new string[] { "Wallet" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(WalletResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetWalletById)]
        [Authorize]
        public async Task<IActionResult> GetWalletByIdAsync(
            [FromRoute(Name = "id")] string id)
        {
            WalletResponse response = await _walletService.GetWalletByIdAsync(id);
            return Ok(response);
        }

        /// <param name="address"></param>
        [SwaggerOperation(Summary = "Get Wallet By Address", Tags = new string[] { "Wallet" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(WalletResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetWalletByAddress)]
        [Authorize]
        public async Task<IActionResult> GetWalletByAddressAsync(
            [FromRoute(Name = "address")] string address)
        {

            WalletResponse response = await _walletService.GetWalletByAddressAsync(address);
            return Ok(response);

        }

        /// <param name="id"></param>
        /// <param name="externalId"></param>
        [SwaggerOperation(Summary = "Associate Wallet External Id", Tags = new string[] { "Wallet" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK")]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPatch(CustodyHSMRoutes.AssociateWalletExternalId)]
        [Authorize]
        public async Task<IActionResult> AssociateWalletExternalIdAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "external_id")] string externalId)
        {
            await _walletService.AssociateWalletExternalIdAsync(id, externalId);
            return Ok();

        }

    }
}
