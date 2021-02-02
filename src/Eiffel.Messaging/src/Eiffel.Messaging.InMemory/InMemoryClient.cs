using Eiffel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.InMemory
{
    public class InMemoryClient : IMessageQueueClient
    {
        private readonly Dictionary<string, Subject<dynamic>> _subscriptions;
        private readonly InMemoryClientConfig _config;

        public InMemoryClient(InMemoryClientConfig config)
        {
             _subscriptions = new Dictionary<string, Subject<dynamic>>();
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public virtual void Consume<TMessage>(string topicName, Action<TMessage> dispatcher) where TMessage : IMessage, new()
        {
            CreateSubscription(topicName);
            _subscriptions[topicName].Subscribe((message) =>
            {
                dispatcher.Invoke(message);
            });
        }

        public virtual Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken) where TMessage : IMessage, new()
        {
            CreateSubscription(topicName);
            _subscriptions[topicName].Subscribe((message) =>
            {
                dispatcher.Invoke(message);
            });
            return Task.CompletedTask;
        }

        public virtual void Produce<TMessage>(string topicName, TMessage message) where TMessage : IMessage, new()
        {
            CreateSubscription(topicName);
            _subscriptions[topicName].OnNext(message);
        }

        public virtual Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken) where TMessage : IMessage, new()
        {
            CreateSubscription(topicName);
             _subscriptions[topicName].OnNext(message);
            return Task.CompletedTask;
        }

        public virtual void Unsubscribe()
        {
            foreach(var subscription in _subscriptions)
            {
                subscription.Value.Dispose();
            }
            _subscriptions.Clear();
        }

        public virtual void Dispose()
        {
            Unsubscribe();
        }

        private void CreateSubscription(string topicName)
        {
            if (!_subscriptions.ContainsKey(topicName))
            {
                _subscriptions.Add(topicName, new Subject<dynamic>());
            }
        }
    }
}
