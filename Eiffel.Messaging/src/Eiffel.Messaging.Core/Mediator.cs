using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Core
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessagingMiddleware _messagingMiddleware;

        public Mediator(IServiceProvider serviceProvider, MiddlewareOptions options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messagingMiddleware = new MessagingMiddleware(options);
        }

        public virtual Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, IMessage
        {
            if (message is ICommand)
                return DispatchMessageAsync(typeof(ICommandHandler<TMessage>), message, cancellationToken);

            if (message is IMessage)
                return DispatchMessageAsync(typeof(IMessageHandler<TMessage>), message, cancellationToken);

            return Task.CompletedTask;
        }

        public virtual Task<TReply> DispatchAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class
        {
            return DispatchMessageAsync(query, cancellationToken);
        }

        public virtual Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent
        {
            return PublishEventAsync(@event, cancellationToken);
        }

        private Task DispatchMessageAsync<TMessage>(Type handlerType, TMessage message, CancellationToken cancellationToken)
            where TMessage : class, IMessage
        {
            var handler = _serviceProvider.GetService(handlerType) as dynamic;

            if (handler == null)
                throw new HandlerCoultNotBeResolvedException($"{handlerType.AssemblyQualifiedName} could not be resolved");

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            if (_messagingMiddleware != null)
                return _messagingMiddleware.Invoke(handler, message, cancellationToken);

            return handler.HandleAsync(message, cancellationToken);
        }

        private Task<TReply> DispatchMessageAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken)
            where TReply : class
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TReply));
            var handler = _serviceProvider.GetService(handlerType) as dynamic;

            if (handler == null)
                throw new HandlerCoultNotBeResolvedException($"{handlerType.AssemblyQualifiedName} could not be resolved");

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            if (_messagingMiddleware != null)
                return _messagingMiddleware.Invoke(handler, query, cancellationToken);

            return handler.HandleAsync((dynamic)query, cancellationToken);
        }

        private Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var handlerType = typeof(IEventHandler<TEvent>);

            var handlers = _serviceProvider.GetServices(handlerType) as IEnumerable<IEventHandler<TEvent>>;
            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                tasks.Add(handler?.HandleAsync(@event, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }
    }
}
