using System;
using System.Runtime.Serialization;

namespace Eiffel.Messaging.Core.Exceptions
{
    public class HandlerCoultNotBeResolvedException : Exception
    {
        public HandlerCoultNotBeResolvedException()
        {
        }

        public HandlerCoultNotBeResolvedException(string message) : base(message)
        {
        }

        public HandlerCoultNotBeResolvedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandlerCoultNotBeResolvedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
