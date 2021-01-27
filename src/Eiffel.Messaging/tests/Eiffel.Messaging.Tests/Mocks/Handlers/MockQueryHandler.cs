﻿using Eiffel.Messaging.Abstractions.Query;
using Eiffel.Messaging.Tests.Mocks.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Messaging.Tests.Mocks.Handlers
{
    public class MockQueryHandler : IQueryHandler<MockQuery, MockQueryResult>
    {
        public virtual Task<MockQueryResult> HandleAsync(MockQuery message, CancellationToken cancellationToken)
        {
            var result = new MockQueryResult();
            result.Items.AddRange(new object[] { new(), new(), new() });
            return Task.FromResult(result);
        }
    }
}
