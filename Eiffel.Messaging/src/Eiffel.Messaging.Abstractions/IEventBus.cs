using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent, new();

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent, new();

        void Subscribe<TEvent>()
            where TEvent : IEvent, new();

        Task SubscribeAsync<TEvent>(CancellationToken cancellationToken)
            where TEvent : IEvent, new();

        void Unsubscribe();
    }
}
