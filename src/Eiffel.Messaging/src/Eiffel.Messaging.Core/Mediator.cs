using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Command;
using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Abstractions.Query;
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

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messagingMiddleware = new MessagingMiddleware(serviceProvider);
        }

        public virtual Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            return DispatchMessageAsync(command, cancellationToken);
        }

        public virtual Task<TReply> DispatchAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class
        {
            return DispatchMessageAsync(query, cancellationToken);
        }

        public virtual Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            return PublishEventAsync(@event, cancellationToken);
        }

        private Task DispatchMessageAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand
        {
            var handlerType = typeof(ICommandHandler<TCommand>);

            var handler = (ICommandHandler<TCommand>)_serviceProvider.GetService(handlerType);
            if (handler == null)
                throw new HandlerCoultNotBeResolvedException($"{handlerType.AssemblyQualifiedName} could not be resolved");

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            return _messagingMiddleware.Handle(handler, command, cancellationToken);
        }

        private Task<TReply> DispatchMessageAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken)
            where TReply : class
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TReply));
            var handler = _serviceProvider.GetService(handlerType) as dynamic;
            if (handler == null)
            {
                throw new HandlerCoultNotBeResolvedException($"{handlerType.AssemblyQualifiedName} could not be resolved");
            }
            return _messagingMiddleware.Handle(handler, query, cancellationToken);
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
