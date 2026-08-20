using Rayls.Custody.HSM.DTO.Enums;
using System;

namespace Rayls.Custody.HSM.Service.Interface.Repositories.Model
{
    public class Wallet
    {

        public Wallet()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public WalletType Type { get; set; }
        public string Data { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string KMSKeyId { get; set; } = string.Empty;
        public int? Index { get; set; }
        public string? ExternalId { get; set; }
    }
}
