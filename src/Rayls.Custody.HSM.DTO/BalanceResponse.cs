using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Rayls.Custody.HSM.DTO.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace Rayls.Custody.HSM.DTO
{
    /// <summary>
    /// Token Response
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class BalanceResponse
    {
        [SwaggerSchema(
        Title = "Balance",
        Description = "",
        Nullable = true)]
        public string Balance { get; set; }
    }
}
