using System;
using System.Runtime.Serialization;

namespace Eiffel.Messaging.Core.Exceptions
{
    public class ConsumeCancelledByUserException : Exception
    {
        public ConsumeCancelledByUserException()
        {
        }

        public ConsumeCancelledByUserException(string message) : base(message)
        {
        }

        public ConsumeCancelledByUserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConsumeCancelledByUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
