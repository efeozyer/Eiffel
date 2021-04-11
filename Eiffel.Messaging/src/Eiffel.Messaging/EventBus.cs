using System;
using System.Threading;
using System.Threading.Tasks;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Attributes;

namespace Eiffel.Messaging
{
    public class EventBus : IEventBus
    {
        private readonly IMessageQueueClient _client;
        private readonly IMediator _mediator;

        public EventBus(IMessageQueueClient client, IMediator mediator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public virtual void Publish<TEvent>(TEvent @event)
            where TEvent : class, new()
        {
            _client.Produce(typeof(TEvent).GetTopic(), @event);
        }

        public virtual Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) 
            where TEvent : class, new()
        {
            return _client.ProduceAsync(typeof(TEvent).GetTopic(), @event, cancellationToken);
        }

        public virtual void Subscribe<TEvent>()
            where TEvent : Event, new()
        {
            _client.Consume<TEvent>(typeof(TEvent).GetTopic(), async (@event) =>
            {
                await _mediator.PublishAsync(@event);
            });
        }

        public virtual Task SubscribeAsync<TEvent>(CancellationToken cancellationToken)
            where TEvent : Event, new()
        {
            return _client.ConsumeAsync<TEvent>(typeof(TEvent).GetTopic(), async (@event) =>
            {
                await _mediator.PublishAsync(@event, cancellationToken);
            }, cancellationToken);
        }

        public virtual void Unsubscribe()
        {
            _client.Unsubscribe();
        }
    }
}
