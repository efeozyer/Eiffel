using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageHandler<in TMessage>
        where TMessage : IMessage
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }

    public interface IMessageHandler<TMessage, TReply> where TMessage : IMessage<TReply>
    {
        Task<TReply> HandleAsync(TMessage message, CancellationToken cancellationToken);
    }

    public abstract class MessageHandler<TMessage>
        where TMessage : class
    {
        public abstract Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}
