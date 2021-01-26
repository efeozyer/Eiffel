using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions.Event
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
