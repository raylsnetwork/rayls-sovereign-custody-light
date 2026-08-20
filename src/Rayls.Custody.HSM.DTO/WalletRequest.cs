using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Rayls.Custody.HSM.DTO.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rayls.Custody.HSM.DTO
{
    /// <summary>
    /// Wallet Request
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class WalletRequest
    {
        [SwaggerSchema(Title = "Type", Description = "", Nullable = false)]
        [Required]
        [BindRequired]
        public WalletType Type { get; set; }

        [SwaggerSchema(Title = "Password", Description = "", Nullable = true)]
        public string? Password { get; set; }

        [SwaggerSchema( Title = "AddressQuantity", Description = "", Nullable = false)]
        [Required]
        [BindRequired]
        public int? AddressQuantity { get; set; }
    }
}