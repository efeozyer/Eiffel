using System;
using System.Threading;
using System.Threading.Tasks;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Attributes;

namespace Eiffel.Messaging.Core
{
    public class EventBus : IEventBus
    {
        private readonly IMessageQueueClient _client;
        private readonly IMessageDispatcher _dispatcher;

        public EventBus(IMessageQueueClient client, IMessageDispatcher dispatcher)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent, new()
        {
            _client.Produce(typeof(TEvent).GetTopic(), @event);
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) 
            where TEvent : IEvent, new()
        {
            return _client.ProduceAsync(typeof(TEvent).GetTopic(), @event, cancellationToken);
        }

        public void Subscribe<TEvent>()
            where TEvent : IEvent, new()
        {
            _client.Consume<TEvent>(typeof(TEvent).GetTopic(), async (@event) =>
            {
                await _dispatcher.PublishAsync(@event);
            });
        }

        public Task SubscribeAsync<TEvent>(CancellationToken cancellationToken)
            where TEvent : IEvent, new()
        {
            return _client.ConsumeAsync<TEvent>(typeof(TEvent).GetTopic(), async (@event) =>
            {
                await _dispatcher.PublishAsync(@event, cancellationToken);
            }, cancellationToken);
        }

        public void Unsubscribe()
        {
            _client.Unsubscribe();
        }
    }
}
