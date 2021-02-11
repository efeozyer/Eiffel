using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IEventHandler<in TEvent> : IMessageHandler<TEvent>
        where TEvent : IEvent
    {
       new Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
