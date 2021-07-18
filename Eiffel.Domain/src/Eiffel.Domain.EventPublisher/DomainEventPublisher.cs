using Eiffel.Domain.Abstractions;
using Eiffel.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Domain.EventPublisher
{
    // TODO: Domain event publisher options
    
    // TODO: Circuit breaker pattern

    // TODO: Optional persist event store state (Consider implementation place)
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IEventBus _eventBus;

        public DomainEventPublisher(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Publish()
        {
            var eventStore = DomainEventStore.Instance;

            do
            {
                eventStore.Process((object @event) =>
                {
                    _eventBus.Publish(@event);

                    eventStore.Commit();
                });

            } while (eventStore.HasAny());
        }

        public Task PublishAsync(CancellationToken cancellationToken)
        {
            var eventStore = DomainEventStore.Instance;

            return Task.Run(() =>
            {
                do
                {
                    eventStore.Process(async (object @event) =>
                    {
                        await _eventBus.PublishAsync(@event, cancellationToken);
                    });

                } while (eventStore.HasAny());
            }).ContinueWith(task =>
            {
                // Domain events importance is high that's why we should ensure publish

                // Commit removes event from concurrent stack
                if (task.IsCompletedSuccessfully)
                {
                    eventStore.Commit();
                }
            });
        }
    }
}
