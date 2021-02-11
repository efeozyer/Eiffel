using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Middlewares
{
    public class ValidationMiddleware : IMessagingMiddleware
    {
        public virtual Task InvokeAsync(object message, CancellationToken cancellationToken)
        {
            if (message is IValidatable msg)
            {
                msg.Validate();
            }
            return Task.CompletedTask;
        }
    }
}
