using System;

namespace Rayls.Custody.HSM.DTO.Auth
{
    public class CustomerStatusResponseMessage
    {
        public Guid customerId { get; set; }
        public bool isBlocked { get; set; }
        public string justification { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}
