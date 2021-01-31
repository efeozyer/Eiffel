using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Middlewares
{
    public class ValidationMiddleware : IMessagingMiddleware
    {
        public virtual Task<TReply> Invoke<TMessage, TReply>(TMessage message, MessagingDelegate<TReply> next) 
            where TMessage : IMessage
        {
            if (message is IValidatable msg)
            {
                msg.Validate();
            }
            return next.Invoke();
        }

        public virtual Task Invoke<TCommand>(TCommand command, MessagingDelegate next)
        {
            if (command is IValidatable cmd)
            {
                cmd.Validate();
            }
            return next.Invoke();
        }
    }
}
