using Azure.Messaging.ServiceBus;
using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using BinaryPack;

namespace Eiffel.Messaging.Azure
{
    public class AzureServiceBusClient : IMessageQueueClient
    {
        private readonly ServiceBusClient _client;
        private readonly ILogger<AzureServiceBusClient> _logger;
        private readonly AzureClientConfig _config;
        private readonly CancellationTokenSource _tokenSource;

        public AzureServiceBusClient(ILogger<AzureServiceBusClient> logger, AzureClientConfig config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new ServiceBusClient(config.ConnectionString);
            _tokenSource = new CancellationTokenSource();
        }

        public virtual void Consume<TMessage>(string topicName, Action<TMessage> dispatcher) where TMessage : IMessage, new()
        {
            var receiver = _client.CreateProcessor(_config.QueueName, topicName);

            receiver.ProcessMessageAsync += async (args) =>
            {
                dispatcher.Invoke(BinaryConverter.Deserialize<TMessage>(args.Message.Body.ToArray()));
                await args.CompleteMessageAsync(args.Message);
                if (_tokenSource.IsCancellationRequested) {
                    await receiver.StopProcessingAsync();
                }
            };

            receiver.ProcessErrorAsync += async (args) =>
            {
                _logger.LogError(args.Exception, "AzureClient consumer error");
                if (_tokenSource.IsCancellationRequested) {
                    await receiver.StopProcessingAsync();
                }
            };

            receiver.StartProcessingAsync().GetAwaiter().GetResult();
        }

        public virtual async Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken) where TMessage : IMessage, new()
        {
            var receiver = _client.CreateProcessor(_config.QueueName, topicName);
            receiver.ProcessMessageAsync += async (args) =>
            {
                dispatcher.Invoke(BinaryConverter.Deserialize<TMessage>(args.Message.Body.ToArray()));
                await args.CompleteMessageAsync(args.Message);
                if (_tokenSource.IsCancellationRequested) {
                    await receiver.StopProcessingAsync();
                }
            };
            receiver.ProcessErrorAsync += async (args) =>
            {
                _logger.LogError(args.Exception, "AzureClient consumer error");
                if (_tokenSource.IsCancellationRequested) {
                    await receiver.StopProcessingAsync();
                }
            };
            await receiver.StartProcessingAsync(cancellationToken);
        }

        public virtual void Produce<TMessage>(string topicName, TMessage message) where TMessage : IMessage, new()
        {
            var sender = _client.CreateSender(topicName);
            sender.SendMessageAsync(new ServiceBusMessage(BinaryConverter.Serialize(message))).GetAwaiter().GetResult();
            sender.DisposeAsync().GetAwaiter().GetResult();
        }

        public virtual async Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken) where TMessage : IMessage, new()
        {
            var sender = _client.CreateSender(topicName);
            await sender.SendMessageAsync(new ServiceBusMessage(BinaryConverter.Serialize(message)));
            await sender.DisposeAsync();
        }

        public virtual void Unsubscribe()
        {
            _tokenSource.Cancel();
        }

        public virtual void Dispose()
        {
            _client?.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
