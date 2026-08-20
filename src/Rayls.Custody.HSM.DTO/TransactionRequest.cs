using Rayls.Custody.HSM.DTO.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Rayls.Custody.HSM.DTO
{
    public class TransactionRequest<T>
    {
        [SwaggerSchema(
        Title = "WalletId",
        Description = "",
        Nullable = false)]
        [Required]
        public string WalletId { get; set; }

        [SwaggerSchema(
        Title = "Password",
        Description = "",
        Nullable = true)]
        public string? Password { get; set; }

        [SwaggerSchema(
        Title = "Transaction",
        Description = "",
        Nullable = false)]
        [Required]
        public T Transaction { get; set; }

        // ChainId/RpcUrl let the caller name the target chain per request. Chains are created at
        // runtime, so the process-wide EVM_JSON_RPC_CHAIN_ID / EVM_JSON_RPC are only a fallback and
        // are empty whenever the service boots before any chain exists. They live on the envelope
        // rather than on the payload because they route the request rather than describe the tx.
        [SwaggerSchema(
        Title = "ChainId",
        Description = "Target chain ID. Falls back to EVM_JSON_RPC_CHAIN_ID when omitted.",
        Nullable = true)]
        public string? ChainId { get; set; }

        [SwaggerSchema(
        Title = "RpcUrl",
        Description = "Target chain JSON-RPC URL. Falls back to EVM_JSON_RPC when omitted.",
        Nullable = true)]
        public string? RpcUrl { get; set; }
    }
}