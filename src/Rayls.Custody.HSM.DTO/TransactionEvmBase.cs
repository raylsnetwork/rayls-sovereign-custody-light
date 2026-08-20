using Swashbuckle.AspNetCore.Annotations;

namespace Rayls.Custody.HSM.DTO
{
    public class TransactionEvmBase
    {
        [SwaggerSchema(
        Title = "To",
        Description = "",
        Nullable = true)]
        public string? To { get; set; }

        [SwaggerSchema(
        Title = "Data",
        Description = "",
        Nullable = true)]
        public string? Data { get; set; }

        [SwaggerSchema(
        Title = "Value",
        Description = "",
        Nullable = true)]
        public string? Value { get; set; }


        [SwaggerSchema(
        Title = "Nonce",
        Description = "",
        Nullable = true)]
        public string? Nonce { get; set; }
    }
}
