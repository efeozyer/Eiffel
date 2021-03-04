using Eiffel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging
{
    internal class MessagingMiddleware
    {
        private readonly List<IMessagingMiddleware> _middlewares;
        public MessagingMiddleware(MessagingMiddlewareOptions options)
        {
            _middlewares = options.GetMiddlewares();
        }

        public Task<TReply> Invoke<TReply>(dynamic handler, IQuery<TReply> message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync((dynamic)message, cancellationToken);
        }

        public Task Invoke(dynamic handler, ICommand message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync((dynamic)message, cancellationToken);
        }

        public Task Invoke(dynamic handler, IMessage message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync((dynamic)message, cancellationToken);
        }

        public void Invoke<TMessage>(TMessage message, Action action)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, default)));
            
            action.Invoke();
        }

        private Task HandleException(Task task)
        {
            return task.ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(task.Exception.InnerException).Throw();
                }
                return task;
            });
        }
    }
}
