using System;
using System.Runtime.Serialization;

namespace Eiffel.Persistence.Core
{
    [Serializable]
    public class CollectionBindingException : Exception
    {
        public CollectionBindingException()
        {
        }

        public CollectionBindingException(string message) : base(message)
        {
        }

        public CollectionBindingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CollectionBindingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
