using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Abstractions
{
    public interface IMediator
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : Event;

        Task<TReply> RequestAsync<TReply>(Query<TReply> query, CancellationToken cancellationToken = default)
            where TReply : class;

        Task<TResult> SendAsync<TResult>(Command command, CancellationToken cancellationToken = default);

        Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : Message;
    }
}
