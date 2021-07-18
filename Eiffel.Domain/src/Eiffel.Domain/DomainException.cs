using System;
using System.Runtime.Serialization;

namespace Eiffel.Domain
{
    [Serializable]
    public abstract class DomainException : Exception
    {
        public abstract string ResourceKey { get; }

        public object[] Parameters { get; }

        protected DomainException()
        {
        }

        protected DomainException(params object[] parameters)
        {
            Parameters = parameters;
        }

        protected DomainException(string message) : base(message)
        {
        }

        protected DomainException(string message, params object[] parameters) : base(string.Format(message, parameters))
        {
            Parameters = parameters;
        }

        protected DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
