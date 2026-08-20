using Microsoft.AspNetCore.Mvc;
using Rayls.Custody.HSM.DTO.Const;
using Rayls.Custody.HSM.DTO.Errors;
using Rayls.Custody.HSM.Repository.PostgreSql;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class HealthController(RaylzDbContext dbContext) : ControllerBase
    {
        [SwaggerOperation(Summary = "Health check", Tags = new string[] { "Health" })]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(200, "OK", typeof(object))]
        [ProducesResponseType(typeof(ApiError), 503)]
        [HttpGet(CustodyHSMRoutes.Health)]
        public async Task<IActionResult> Get()
        {
            var dbReachable = await dbContext.Database.CanConnectAsync();

            if (!dbReachable)
                return StatusCode(503, new { status = "unhealthy", database = "unreachable" });

            return Ok(new { status = "healthy", database = "ok" });
        }
    }
}
