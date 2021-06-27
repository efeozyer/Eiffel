using System;
using System.Runtime.Serialization;

namespace Eiffel.Persistence.Tenancy
{
    [Serializable]
    public class TenantRegistrationExpiredException : Exception
    {
        public TenantRegistrationExpiredException()
        {
        }

        public TenantRegistrationExpiredException(string message) : base(message)
        {
        }

        public TenantRegistrationExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TenantRegistrationExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
