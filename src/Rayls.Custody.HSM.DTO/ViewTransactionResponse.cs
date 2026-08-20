using Rayls.Custody.HSM.DTO.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Rayls.Custody.HSM.DTO
{
    public class ViewTransactionResponse
    {
        [SwaggerSchema(
        Title = "Data",
        Description = "",
        Nullable = false)]
        [Required]
        public string Data { get; set; }

    }
}