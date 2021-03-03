using System;
using System.Runtime.Serialization;

namespace Eiffel.Messaging.Core.Exceptions
{
    [Serializable]
    public class ProduceFailedException : Exception
    {
        public ProduceFailedException()
        {
        }

        public ProduceFailedException(string message) : base(message)
        {
        }

        public ProduceFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProduceFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
