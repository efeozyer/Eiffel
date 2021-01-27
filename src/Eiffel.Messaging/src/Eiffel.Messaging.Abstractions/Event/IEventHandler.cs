namespace Eiffel.Messaging.Abstractions.Event
{
    public interface IEventHandler<in TEvent> : IMessageHandler<TEvent>
        where TEvent : IEvent
    {
    }
}
