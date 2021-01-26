using Eiffel.Messaging.Abstractions.Command; 
using Eiffel.Messaging.Abstractions.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMessageDispatcher
    {
        Task<TReply> DispatchAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class;
        
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand;

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
