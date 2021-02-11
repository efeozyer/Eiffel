using Eiffel.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Middlewares
{
    public class LoggingMiddleware : IMessagingMiddleware
    {
        public virtual Task InvokeAsync(object message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
