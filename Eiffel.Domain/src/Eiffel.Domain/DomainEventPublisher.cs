//using Eiffel.Messaging.Abstractions;

//namespace Eiffel.Domain
//{
//    public class DomainEventPublisher
//    {
//        private readonly IEventBus _eventBus;

//        public DomainEventPublisher(IEventBus eventBus)
//        {
//            _eventBus = eventBus ?? throw new System.ArgumentNullException(nameof(eventBus));
//        }

//        public void Publish()
//        {
//            var eventStore = DomainEventStore.Instance;
//            do
//            {
//                eventStore.Process(async (object @event) =>
//                {
//                    await _eventBus.PublishAsync(@event, default);
//                });
//            } while (eventStore.HasAny());
//        }
//    }
//}
