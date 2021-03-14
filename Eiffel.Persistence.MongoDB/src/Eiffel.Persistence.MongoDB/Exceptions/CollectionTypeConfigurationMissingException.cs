using System;
using System.Runtime.Serialization;

namespace Eiffel.Persistence.MongoDB.Exceptions
{
    [Serializable]
    public class CollectionTypeConfigurationMissingException : Exception
    {
        public CollectionTypeConfigurationMissingException()
        {
        }

        public CollectionTypeConfigurationMissingException(string message) : base(message)
        {
        }

        public CollectionTypeConfigurationMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CollectionTypeConfigurationMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
