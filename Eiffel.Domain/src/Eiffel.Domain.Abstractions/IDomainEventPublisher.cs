using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Domain.Abstractions
{
    /// <summary>
    /// Domain event publisher
    /// </summary>
    public interface IDomainEventPublisher
    {
        void Publish();

        Task PublishAsync(CancellationToken cancellationToken);
    }
}
