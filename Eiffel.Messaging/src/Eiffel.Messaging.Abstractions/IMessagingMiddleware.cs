using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessagingMiddleware
    {
        Task InvokeAsync(object message, CancellationToken cancellationToken);
    }
}
