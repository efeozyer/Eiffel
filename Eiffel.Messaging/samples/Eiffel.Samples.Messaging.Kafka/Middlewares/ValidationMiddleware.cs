using Eiffel.Messaging.Abstractions;
using Eiffel.Samples.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Messaging.Kafka.Middlewares
{
    public class ValidationMiddleware : IMessagingMiddleware
    {
        public Task InvokeAsync(object message, CancellationToken cancellationToken)
        {
            if (message is IValidatable msg)
            {
                msg.Validate();
            }

            // TODO: Validate message
            return Task.CompletedTask;
        }
    }
}
