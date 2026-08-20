using System;

namespace Rayls.Custody.HSM.Service
{
    public class CustodyHSMAPIValidationException : Exception
    {
        private const int invalidRequest = 400;

        public int StatusCode { get; }
        public string DetailedMessage { get; }

        public CustodyHSMAPIValidationException(string message, int statusCode = invalidRequest, string detailedMessage = "")
            : base(message)
        {
            StatusCode = statusCode;
            DetailedMessage = detailedMessage;
        }
    }
}
