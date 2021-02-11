using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMediator
    {
        Task<TReply> DispatchAsync<TReply>(IQuery<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class;
        
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand;

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
