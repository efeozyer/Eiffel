using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Core
{
    internal class MessagingMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        public MessagingMiddleware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<TReply> Handle<TReply>(dynamic handler, IQuery<TReply> message, CancellationToken cancellationToken)
        {
            Task<TReply> handleAsync() => handler.HandleAsync((dynamic)message, cancellationToken);
            var middlewares = _serviceProvider.GetServices(typeof(IMessagingMiddleware)) as IEnumerable<IMessagingMiddleware>;

            return middlewares
                .Reverse()
                .Aggregate((MessagingDelegate<TReply>)handleAsync, (next, middleware) => () => HandleException<TReply>(middleware.Invoke((dynamic)message, next)))();
        }

        public Task Handle<TCommand>(ICommandHandler<TCommand> handler, ICommand message, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            Task handleAsync() => handler.HandleAsync((dynamic)message, cancellationToken);
            var middlewares = _serviceProvider.GetServices(typeof(IMessagingMiddleware)) as IEnumerable<IMessagingMiddleware>;

            return middlewares
                .Reverse()
                .Aggregate((MessagingDelegate)handleAsync, (next, middleware) => () => HandleException(middleware.Invoke((dynamic)message, next)))();
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

        private Task<TResult> HandleException<TResult>(Task<TResult> task)
        {
            return task.ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(task.Exception.InnerException).Throw();
                }
                return task.Result;
            });
        }
    }
}
