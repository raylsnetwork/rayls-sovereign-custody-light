using System;

namespace Rayls.Custody.HSM.DTO.Errors
{
    /// <summary>
    /// Thrown when the presented API key does not match the configured one. Distinguishes a
    /// genuine credential mismatch from an internal fault, so the two are not reported alike.
    /// </summary>
    public class UnauthorizedApiKeyException : Exception
    {
        public UnauthorizedApiKeyException()
            : base("The presented API key is not valid.")
        {
        }
    }
}
