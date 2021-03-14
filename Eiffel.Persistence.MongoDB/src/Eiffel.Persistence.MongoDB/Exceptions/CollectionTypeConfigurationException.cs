using System;
using System.Runtime.Serialization;

namespace Eiffel.Persistence.MongoDB.Exceptions
{
    [Serializable]
    public class CollectionTypeConfigurationException : Exception
    {
        public CollectionTypeConfigurationException()
        {
        }

        public CollectionTypeConfigurationException(string message) : base(message)
        {
        }

        public CollectionTypeConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CollectionTypeConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
