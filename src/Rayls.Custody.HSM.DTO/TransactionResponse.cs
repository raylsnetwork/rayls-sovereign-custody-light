using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Rayls.Custody.HSM.DTO
{
    public class TransactionResponse
    {
        [SwaggerSchema(
        Title = "TxHash",
        Description = "",
        Nullable = true)]
        public string TxHash { get; set; }
    }
}