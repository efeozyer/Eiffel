using System;
using System.Runtime.Serialization;

namespace Eiffel.Messaging.Exceptions
{
    public class HandlerCouldNotBeResolvedException : Exception
    {
        public HandlerCouldNotBeResolvedException()
        {
        }

        public HandlerCouldNotBeResolvedException(string message) : base(message)
        {
        }

        public HandlerCouldNotBeResolvedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandlerCouldNotBeResolvedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
