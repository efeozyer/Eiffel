using Eiffel.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Pipelines
{
    public class MockAuditLoggingPipeline : IPipelinePostProcessor
    {
        public virtual Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
