using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IMessageQueueClient _client;
        private readonly IMediator _mediator;

        public MessageBus(IMessageQueueClient client, IMediator mediator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class, new()
        {
            _client.Produce(typeof(TMessage).GetTopic(), message);
        }

        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new()
        {
            return _client.ProduceAsync(typeof(TMessage).GetTopic(), message);
        }

        public void Subscribe<TMessage>()
            where TMessage : Message, new()
        {
            _client.Consume<TMessage>(typeof(TMessage).GetTopic(), async (message) =>
            {
                await _mediator.DispatchAsync(message, default);
            });
        }

        public Task SubscribeAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : Message, new()
        {
            return _client.ConsumeAsync<TMessage>(typeof(TMessage).GetTopic(), async (message) =>
            {
                await _mediator.DispatchAsync(message, cancellationToken);
            }, cancellationToken);
        }

        public void Unsubscribe()
        {
            _client.Unsubscribe();
        }
    }
}
