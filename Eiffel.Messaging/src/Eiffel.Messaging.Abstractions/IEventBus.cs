﻿using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : class, new();

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, new();

        void Subscribe<TEvent>()
            where TEvent : class, IEvent, new();

        Task SubscribeAsync<TEvent>(CancellationToken cancellationToken)
            where TEvent : class, IEvent, new();

        void Unsubscribe();
    }
}
