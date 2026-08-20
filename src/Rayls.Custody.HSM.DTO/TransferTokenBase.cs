using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Rayls.Custody.HSM.DTO
{
    public class TransferTokenBase
    {
        [SwaggerSchema(
        Title = "TokenId",
        Description = "",
        Nullable = false)]
        [Required]
        public string TokenId { get; set; }

        [SwaggerSchema(
        Title = "To",
        Description = "",
        Nullable = false)]
        [Required]
        public string To { get; set; }

        [SwaggerSchema(
        Title = "Value",
        Description = "",
        Nullable = false)]
        [Required]
        public string Value { get; set; }
    }
}
