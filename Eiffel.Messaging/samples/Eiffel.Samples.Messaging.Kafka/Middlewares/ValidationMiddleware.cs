using Eiffel.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Samples.Messaging.Kafka.Middlewares
{
    public class ValidationMiddleware : IMessagingMiddleware
    {
        public Task InvokeAsync(object message, CancellationToken cancellationToken)
        {
            // TODO: Validate message
            return Task.CompletedTask;
        }
    }
}
