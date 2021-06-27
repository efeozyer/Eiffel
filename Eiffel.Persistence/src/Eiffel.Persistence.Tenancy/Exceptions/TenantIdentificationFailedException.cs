using System;
using System.Runtime.Serialization;

namespace Eiffel.Persistence.Tenancy
{
    [Serializable]
    public class TenantIdentificationFailedException : Exception
    {
        public TenantIdentificationFailedException()
        {
        }

        public TenantIdentificationFailedException(string message) : base(message)
        {
        }

        public TenantIdentificationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TenantIdentificationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
