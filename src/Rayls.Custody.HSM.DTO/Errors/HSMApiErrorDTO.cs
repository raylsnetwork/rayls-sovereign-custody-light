using Newtonsoft.Json;
using Rayls.Custody.HSM.DTO.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rayls.Custody.HSM.DTO
{
    public sealed class HSMApiErrorDTO
    {
        public DateTime Timestamp { get; }
        public string TraceKey { get; }
        public IEnumerable<HSMErrorDTO> Errors { get; private set; }

        public HSMApiErrorDTO(DateTime timestamp, string traceKey)
            : this(timestamp, traceKey, default(IEnumerable<HSMErrorDTO>))
        {
        }

        [JsonConstructor]
        public HSMApiErrorDTO(DateTime timestamp, string traceKey, IEnumerable<HSMErrorDTO> errors)
        {
            Timestamp = timestamp;
            TraceKey = traceKey;
            Errors = errors;
        }

        public HSMApiErrorDTO(DateTime timestamp, string traceKey, HSMErrorDTO error)
            : this(timestamp, traceKey, new List<HSMErrorDTO> { error }.AsEnumerable())
        {
        }

        public HSMApiErrorDTO(DateTime timestamp, string traceKey, string errorCode, string errorMessage)
            : this(timestamp, traceKey, new HSMErrorDTO(errorCode, errorMessage))
        {
        }

        public void AddErrors(IEnumerable<HSMErrorDTO> errors) => Errors = errors;

        public void AddError(HSMErrorDTO error) => AddErrors(new List<HSMErrorDTO> { error }.AsEnumerable());

        public void AddError(string errorCode, string errorMessage) => AddError(new HSMErrorDTO(errorCode, errorMessage));
    }
}
