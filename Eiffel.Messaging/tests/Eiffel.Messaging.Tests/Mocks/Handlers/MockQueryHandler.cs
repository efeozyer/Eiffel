using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockQueryHandler : QueryHandler<MockQuery, MockQueryResult>
    {
        public override Task<MockQueryResult> HandleAsync(MockQuery query, CancellationToken cancellationToken = default)
        {
            var result = new MockQueryResult();
            result.Items.AddRange(new object[] { new(), new(), new() });
            return Task.FromResult(result);
        }
    }
}
