using Autofac;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging
{
    public class Mediator : IMediator
    {
        private readonly ILifetimeScope _lifetimeScope;

        public Mediator(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public virtual Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event
        {
            var handlerType = typeof(IEnumerable<Abstractions.EventHandler<TEvent>>);
            var handlers = _lifetimeScope.Resolve(handlerType) as IEnumerable<Abstractions.EventHandler<TEvent>>;
            if (handlers == null || !handlers.Any())
            {
                throw new HandlerCouldNotBeResolvedException($"{@event.GetType().Name} handler could not be resolved");
            }

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                tasks.Add(handler?.HandleAsync(@event, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        public virtual Task<TReply> RequestAsync<TReply>(Query<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class
        {
            var handlerType = typeof(QueryHandler<,>).MakeGenericType(query.GetType(), typeof(TReply));
            return DispatchAsync<Query<TReply>, Task<TReply>>(query, handlerType, cancellationToken);
        }

        public virtual Task<TResult> SendAsync<TResult>(Command command, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(CommandHandler<>).MakeGenericType(command.GetType());
            return DispatchAsync<Command, Task<TResult>>(command, handlerType, cancellationToken);
        }

        public virtual Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : Message
        {
            var handlerType = typeof(MessageHandler<>).MakeGenericType(message.GetType());
            return DispatchAsync<TMessage, Task>(message, handlerType, cancellationToken);
        }

        private TResult DispatchAsync<TMessage, TResult>(TMessage message, Type handlerType, CancellationToken cancellationToken)
        {
            var handler = _lifetimeScope.ResolveOptional(handlerType);

            if (handler == null)
                throw new HandlerCouldNotBeResolvedException($"{message.GetType().Name} handler could not be resolved");

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            var handleMethod = handler.GetType().GetMethod("HandleAsync");

            if (handleMethod == null)
                throw new MissingMethodException("HandleAsync");

            var preProcessors = _lifetimeScope.ResolveOptional<IEnumerable<IPipelinePreProcessor>>()?.ToList();
            foreach(var processor in preProcessors ?? Enumerable.Empty<IPipelinePreProcessor>())
            {
                HandleException(processor.ProcessAsync(message, cancellationToken)).ConfigureAwait(false);
            }

            var result = (TResult)handleMethod.Invoke(handler, new object[] { message, cancellationToken });

            var postProcessors = _lifetimeScope.ResolveOptional<IEnumerable<IPipelinePostProcessor>>()?.ToList();
            foreach (var processor in postProcessors ?? Enumerable.Empty<IPipelinePostProcessor>())
            {
                HandleException(processor.ProcessAsync(message, cancellationToken)).ConfigureAwait(false);
            }

            return result;
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
