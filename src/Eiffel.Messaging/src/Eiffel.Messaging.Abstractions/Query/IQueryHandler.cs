namespace Eiffel.Messaging.Abstractions.Query
{
    public interface IQueryHandler<TQuery, TReply> : IMessageHandler<TQuery, TReply>
        where TQuery : IMessage<TReply> 
    {
    }
}
