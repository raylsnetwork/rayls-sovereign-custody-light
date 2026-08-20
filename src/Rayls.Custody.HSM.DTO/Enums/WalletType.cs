using System.ComponentModel;

namespace Rayls.Custody.HSM.DTO.Enums
{
    public enum WalletType
    {
        [Description("KEYSTORE_V3")]
        KEYSTORE_V3,
        [Description("AWS_KMS")]
        AWS_KMS,
        //[Description("GOOGLE_KMS")]
        //GOOGLE_KMS,
        //[Description("AZURE_VAULT")]
        //AZURE_VAULT,
        //[Description("DINAMO")]
        //DINAMO
    }
}
