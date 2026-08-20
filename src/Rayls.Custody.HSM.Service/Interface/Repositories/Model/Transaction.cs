using Rayls.Custody.HSM.DTO.Enums;

namespace Rayls.Custody.HSM.Service.Interface.Repositories.Model
{
    public class Transaction
    {
        public string Id { get; set; }
        public OperationType OperationType { get; set; }
        public string? From { get; set; }
        public string? TxHash { get; set; }
    }
}