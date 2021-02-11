using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageBus
    {
        void Send<TMessage>(TMessage message)
            where TMessage : class,  new();

        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new();

        void Subscribe<TMessage>()
            where TMessage : class, IMessage, new();

        Task SubscribeAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : class, IMessage, new();

        void Unsubscribe();
    }
}
