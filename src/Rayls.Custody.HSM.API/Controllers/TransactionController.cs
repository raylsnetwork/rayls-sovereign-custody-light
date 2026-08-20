using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Const;
using Rayls.Custody.HSM.Service.Interface.Services;
using Rayls.Custody.HSM.DTO.Errors;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <param name="request"></param>
        [SwaggerOperation(
        Summary = "Create Transaction",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TransactionResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.CreateTransaction)]
        [Authorize]
        public async Task<IActionResult> CreateTransactionAsync(
            [FromBody] TransactionRequest<TransactionEvmBase> request)
        {

            return Ok(await _transactionService.CreateTransactionAsync(request));

        }

        /// <param name="request"></param>
        [SwaggerOperation(
        Summary = "View Transaction",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TransactionResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.ViewTransaction)]
        [Authorize]
        public async Task<IActionResult> ViewTransactionAsync(
            [FromBody] ViewTransactionRequest request)
        {

            return Ok(await _transactionService.ViewTransactionAsync(request));

        }


        /// <param name="request"></param>
        [SwaggerOperation(
        Summary = "Create Transaction (Transfer Token)",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TransactionResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.CreateTransactionToTransferToken)]
        [Authorize]
        public async Task<IActionResult> CreateTransactionAsync(
            [FromBody] TransactionRequest<TransferTokenBase> request)
        {

            return Ok(await _transactionService.CreateTransactionAsync(request));

        }

        /// <param name="request"></param>
        [SwaggerOperation(
        Summary = "Create Transaction (Transfer Token Bridge)",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(TransactionResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpPost(CustodyHSMRoutes.CreateTransactionToTransferTokenBridge)]
        [Authorize]
        public async Task<IActionResult> CreateTransactionAsync(
            [FromBody] TransactionRequest<TransferTokenBridgeBase> request)
        {
            return Ok(await _transactionService.CreateTransactionAsync(request));

        }

        /// <param name="hash"></param>
        [SwaggerOperation(
        Summary = "Get Transaction By Id",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(WalletResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetTransactionByHash)]
        [Authorize]
        public async Task<IActionResult> GetTransactionByHashAsync(
            [FromRoute(Name = "hash")] string hash)
        {

            return Ok(await _transactionService.GetTransactionByHash(hash));

        }

        /// <param name="hash"></param>
        [SwaggerOperation(
        Summary = "Get Transaction Receipt By Id",
        Description = "",
        Tags = new string[] { "Transaction" })]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(WalletResponse))]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
        [HttpGet(CustodyHSMRoutes.GetTransactionReceiptByHash)]
        [Authorize]
        public async Task<IActionResult> GetTransactionReceiptByHashAsync(
            [FromRoute(Name = "hash")] string hash)
        {

            return Ok(await _transactionService.GetTransactionReceiptByHash(hash));

        }

    }
}
