using System.ComponentModel;

namespace Rayls.Custody.HSM.DTO.Enums
{
    public enum OperationType
    {
        [Description("TRANSACTION")]
        TRANSACTION,
        [Description("TRANSFER_TOKEN")]
        TRANSFER_TOKEN,
        [Description("TRANSFER_TOKEN_BRIDGE")]
        TRANSFER_TOKEN_BRIDGE,
    }
}
