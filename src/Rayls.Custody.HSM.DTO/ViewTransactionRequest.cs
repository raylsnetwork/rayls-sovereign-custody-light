using Rayls.Custody.HSM.DTO.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Rayls.Custody.HSM.DTO
{
    public class ViewTransactionRequest
    {
        [SwaggerSchema(
        Title = "FromAddress",
        Description = "",
        Nullable = false)]
        [Required]
        public string FromAddress { get; set; }

        [SwaggerSchema(
        Title = "ContractAddress",
        Description = "",
        Nullable = false)]
        [Required]
        public string ContractAddress { get; set; }

        [SwaggerSchema(
        Title = "Data",
        Description = "",
        Nullable = false)]
        [Required]
        public string Data { get; set; }

    }
}