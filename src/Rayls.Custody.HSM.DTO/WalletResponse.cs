using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Rayls.Custody.HSM.DTO.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace Rayls.Custody.HSM.DTO
{
    /// <summary>
    /// Wallet Response
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class WalletResponse
    {
        [SwaggerSchema(
        Title = "Id",
        Description = "",
        Nullable = true)]
        public string? Id { get; set; }

        [SwaggerSchema(
        Title = "Type",
        Description = "",
        Nullable = true )]
        public WalletType Type { get; set; }

        [SwaggerSchema(
        Title = "Address",
        Description = "",
        Nullable = true)]
        public string Address { get; set; }

        [SwaggerSchema(
        Title = "PublicKey",
        Description = "",
        Nullable = true)]
        public string PublicKey { get; set; }
    }
}
