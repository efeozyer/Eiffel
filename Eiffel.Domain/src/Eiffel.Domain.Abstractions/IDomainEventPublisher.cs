using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Domain.Abstractions
{
    public interface IDomainEventPublisher
    {
        void Publish();

        Task PublishAsync(CancellationToken cancellationToken);
    }
}
