namespace Rayls.Custody.HSM.DTO.Errors
{
    public sealed class HSMErrorDTO
    {
        public string Code { get; }
        public string Message { get; }

        public HSMErrorDTO(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
