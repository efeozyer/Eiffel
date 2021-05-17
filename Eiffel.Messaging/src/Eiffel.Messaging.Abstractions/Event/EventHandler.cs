using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public abstract class EventHandler<TEvent>
        where TEvent : Event
    {
        public abstract Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}
