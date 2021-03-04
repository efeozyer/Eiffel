using System;
using System.Threading;
using System.Threading.Tasks;
using BinaryPack;
using Confluent.Kafka;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Exceptions;
using Microsoft.Extensions.Logging;

namespace Eiffel.Messaging.Kafka
{
    public class KafkaClient : IMessageQueueClient
    {
        private readonly ILogger<KafkaClient> _logger;
        private readonly IProducer<Null, byte[]> _producer;
        private readonly IConsumer<Null, byte[]> _consumer;
        private readonly CancellationTokenSource _tokenSource;
        private readonly KafkaClientConfig _config;

        public KafkaClient(ILogger<KafkaClient> logger, KafkaClientConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _producer = new ProducerBuilder<Null, byte[]>(config.ProducerConfig).Build();
            _consumer = new ConsumerBuilder<Null, byte[]>(config.ConsumerConfig).Build();
            _tokenSource = new CancellationTokenSource();
        }

        public virtual async Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken = default)
            where TMessage : class, new()
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
                        var result = _consumer.Consume();
                        if (result?.Message?.Value?.Length > 0)
                        {
                            var msg = BinaryConverter.Deserialize<TMessage>(result.Message.Value);
                            dispatcher.Invoke(msg);
                        }

                        if (!(_config.ConsumerConfig.EnableAutoCommit ?? false))
                        {
                            _consumer.Commit(result);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "An error occourd KafkaClient consumer");
                    }
                }
            }, cancellationToken);
        }

        public virtual void Consume<TMessage>(string topicName, Action<TMessage> dispatcher)
            where TMessage : class, new()
        {
            _consumer.Subscribe(topicName);
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        throw new ConsumeCancelledByUserException();
                    }

                    try
                    {
                        var result = _consumer.Consume();
                        if (result?.Message?.Value?.Length > 0)
                        {
                            var msg = BinaryConverter.Deserialize<TMessage>(result.Message.Value);
                            dispatcher.Invoke(msg);
                        }

                        if (!(_config.ConsumerConfig.EnableAutoCommit ?? false))
                        {
                            _consumer.Commit(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occourd KafkaClient consumer");
                    }
                }
            });
        }

        public virtual async Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new()
        {
            var deliveryResult = await _producer.ProduceAsync(topicName, new Message<Null, byte[]>
            {
                Value = BinaryConverter.Serialize(message),
                Timestamp = new Timestamp(DateTime.UtcNow)
            }, cancellationToken);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new ProduceFailedException($"Message cannot be devlivered to broker {topicName}");
            }
        }

        public virtual void Produce<TMessage>(string topicName, TMessage message)
            where TMessage : class, new()
        {
            _producer.Produce(topicName, new Message<Null, byte[]>
            {
                Value = BinaryConverter.Serialize(message),
                Timestamp = new Timestamp(DateTime.UtcNow)
            }, (deliveryReport) =>
            {
                if (deliveryReport.Status == PersistenceStatus.NotPersisted)
                {
                    throw new ProduceFailedException($"Message cannot be devlivered to broker {topicName}");
                }
            });
        }

        public virtual void Unsubscribe()
        {
            _tokenSource.Cancel();
            _consumer.Unsubscribe();
        }

        public virtual void Dispose()
        {
            _producer?.Dispose();
            _consumer?.Dispose();
        }
    }
}
