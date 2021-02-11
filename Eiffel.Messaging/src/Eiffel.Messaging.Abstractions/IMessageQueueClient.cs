using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageQueueClient : IDisposable
    {
        Task ProduceAsync<TMessage>(string topicName, TMessage message, CancellationToken cancellationToken) where TMessage : IMessage, new();
        void Produce<TMessage>(string topicName, TMessage message) where TMessage : IMessage, new();
        Task ConsumeAsync<TMessage>(string topicName, Action<TMessage> dispatcher, CancellationToken cancellationToken) where TMessage : IMessage, new();
        void Consume<TMessage>(string topicName, Action<TMessage> dispatcher) where TMessage : IMessage, new();
        void Unsubscribe();
    }
}
