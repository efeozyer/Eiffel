using Eiffel.Messaging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.InMemory
{
    public class InMemoryClient : IMessageQueueClient
    {
        private readonly ConcurrentDictionary<string, Subject<dynamic>> _subscriptions;
        private readonly InMemoryClientConfig _config;

        public InMemoryClient(InMemoryClientConfig config)
        {
             _subscriptions = new ConcurrentDictionary<string, Subject<dynamic>>();
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public virtual void Consume<TMessage>(string topicName, Action<TMessage> dispatcher) 
            where TMessage : class, new()
        {
            CreateSubscription(topicName);
            _subscriptions[topicName].Subscribe((message) =>
            {
                dispatcher.Invoke(message);
            });
        }

        public virtual Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken = default)
            where TMessage : class, new()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            CreateSubscription(topicName);
            _subscriptions[topicName].Subscribe((message) =>
            {
                dispatcher.Invoke(message);
            });
            return Task.CompletedTask;
        }

        public virtual void Produce<TMessage>(string topicName, TMessage message) 
            where TMessage : class, new()
        {
            CreateSubscription(topicName);
            _subscriptions[topicName].OnNext(message);
        }

        public virtual Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : class, new()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

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
                var result = _subscriptions.TryAdd(topicName, new Subject<dynamic>());
                if (!result)
                    throw new AccessViolationException();
            }
        }
    }
}
