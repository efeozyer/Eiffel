using Eiffel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Core
{
    internal class MessagingMiddleware
    {
        private readonly List<IMessagingMiddleware> _middlewares;
        public MessagingMiddleware(List<IMessagingMiddleware> middlewares)
        {
            _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
        }

        public Task<TReply> Handle<TReply>(dynamic handler, object message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync(message, cancellationToken);
        }

        public Task Handle(dynamic handler, ICommand message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync((dynamic)message, cancellationToken);
        }

        public Task Handle(dynamic handler, IMessage message, CancellationToken cancellationToken)
        {
            _middlewares
                .ForEach(async x => await HandleException(x.InvokeAsync(message, cancellationToken)));

            return handler.HandleAsync((dynamic)message, cancellationToken);
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
