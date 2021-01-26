using System;
using System.Threading;
using System.Threading.Tasks;
using BinaryPack;
using Confluent.Kafka;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Eiffel.Messaging.Providers.Kafka
{
    public class KafkaClient : IMessageQueueClient
    {
        private readonly ILogger<KafkaClient> _logger;
        private readonly IProducer<Null, byte[]> _producer;
        private readonly IConsumer<Null, byte[]> _consumer;
        private readonly CancellationTokenSource _tokenSource;

        public KafkaClient(ILogger<KafkaClient> logger, KafkaClientConfig config)
        {
            _ = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _producer = new ProducerBuilder<Null, byte[]>(config.ProducerConfig).Build();
            _consumer = new ConsumerBuilder<Null, byte[]>(config.ConsumerConfig).Build();
            _tokenSource = new CancellationTokenSource();
        }

        public virtual async Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken)
            where TMessage : IMessage, new()
        {
            await Task.Factory.StartNew(() =>
            {
                _consumer.Subscribe(topicName);
                while (true)
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        throw new ConsumeCancelledByUserException();
                    }

                    try
                    {
                        var message = _consumer.Consume().Message;
                        if (message?.Value?.Length > 0)
                        {
                            var msg = BinaryConverter.Deserialize<TMessage>(message.Value);
                            dispatcher.Invoke(msg);
                            _consumer.Commit();
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "An error occourd KafkaClient consumer");
                    }
                }
            }, cancellationToken);
        }

        public void Consume<TMessage>(string topicName, Action<TMessage> dispatcher)
            where TMessage : IMessage, new()
        {
            _consumer.Subscribe(topicName);
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_tokenSource.IsCancellationRequested) {
                        throw new ConsumeCancelledByUserException();
                    }

                    var message = _consumer.Consume().Message;
                    if (message?.Value?.Length > 0)
                    {
                        var msg = BinaryConverter.Deserialize<TMessage>(message.Value);
                        dispatcher.Invoke(msg);
                        _consumer.Commit();
                    }
                }
            });
        }

        public virtual async Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken)
            where TMessage : IMessage, new()
        {
            _ = await _producer.ProduceAsync(topicName, new Message<Null, byte[]>
            {
                Value = BinaryConverter.Serialize(message)
            }, cancellationToken);
        }

        public virtual void Produce<TMessage>(string topicName, TMessage message)
            where TMessage : IMessage, new()
        {
            _producer.Produce(topicName, new Message<Null, byte[]>
            {
                Value = BinaryConverter.Serialize(message),
                Timestamp = new Timestamp(DateTime.UtcNow)
            });
        }

        public virtual void Dispose()
        {
            _producer?.Dispose();
            _consumer?.Dispose();
        }

        public virtual void Unsubscribe()
        {
            _tokenSource.Cancel();
            _consumer.Unsubscribe();
        }
    }
}
