using Eiffel.Messaging.Abstractions;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Middlewares
{
    public class LoggingMiddleware : IMessagingMiddleware
    {
        public virtual Task<TReply> Invoke<TMessage, TReply>(TMessage message, MessagingDelegate<TReply> next) 
            where TMessage : IMessage
        {
            return next.Invoke();
        }

        public virtual Task Invoke<TCommand>(TCommand command, MessagingDelegate next)
        {
            return next.Invoke();
        }
    }
}
