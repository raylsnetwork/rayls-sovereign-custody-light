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
    public class TokenResponse
    {
        [SwaggerSchema(
        Title = "Id",
        Description = "",
        Nullable = true)]
        public string Id { get; set; }

        [SwaggerSchema(
        Title = "Address",
        Description = "",
        Nullable = true)]
        public string Address { get; set; }

        [SwaggerSchema(
        Title = "Decimals",
        Description = "",
        Nullable = true)]
        public int Decimals { get; set; }

        [SwaggerSchema(
        Title = "Name",
        Description = "",
        Nullable = true)]
        public string Name { get; set; }

        [SwaggerSchema(
        Title = "Symbol",
        Description = "",
        Nullable = true)]
        public string Symbol { get; set; }

    }
}
