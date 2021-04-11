using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : class, new();

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, new();

        void Subscribe<TEvent>()
            where TEvent : Event, new();

        Task SubscribeAsync<TEvent>(CancellationToken cancellationToken)
            where TEvent : Event, new();

        void Unsubscribe();
    }
}
