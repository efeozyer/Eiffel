using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageQueueClient : IDisposable
    {
        void Produce<TMessage>(string topicName, TMessage message)
          where TMessage : class, new();

        Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new();

        void Consume<TMessage>(string topicName, Action<TMessage> dispatcher)
            where TMessage : class, new();

        Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken = default)
            where TMessage : class, new();

        void Unsubscribe();
    }
}
