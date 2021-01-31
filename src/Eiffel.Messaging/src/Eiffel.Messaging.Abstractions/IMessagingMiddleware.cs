using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public delegate Task<TReply> MessagingDelegate<TReply>();
    public delegate Task MessagingDelegate();

    public interface IMessagingMiddleware
    {
        Task<TReply> Invoke<TMessage, TReply>(TMessage message, MessagingDelegate<TReply> next)
            where TMessage : IMessage;

        Task Invoke<TCommand>(TCommand command, MessagingDelegate next);
    }

    public interface IMessageBusMiddleware
    {

    }
}
