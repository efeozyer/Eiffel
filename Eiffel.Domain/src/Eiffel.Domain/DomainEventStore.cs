using System;
using System.Collections.Concurrent;

namespace Eiffel.Domain
{
    public sealed class DomainEventStore
    {
        private static readonly object _lock = new();

        private static DomainEventStore instance = null;

        private static readonly ConcurrentStack<DomainEvent> _domainEvents = new ConcurrentStack<DomainEvent>();

        public static DomainEventStore Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new DomainEventStore();
                        }
                    }
                }
                return instance;
            }
        }

        public bool HasAny()
        {
            return _domainEvents.IsEmpty;
        }

        public void Add<TEvent>(TEvent @event) where TEvent : DomainEvent
        {
            _domainEvents.Push(@event);
        }

        public void Process(Action<object> action)
        {
            bool result = _domainEvents.TryPeek(out DomainEvent @event);

            if (!result)
            {
                return;
            }

            action.Invoke(@event);
        }

        public void Commit()
        {
            _domainEvents.TryPop(out _);
        }
    }
}
