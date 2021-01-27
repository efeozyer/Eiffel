using Eiffel.Messaging.Abstractions.Query;
using System.Collections.Generic;

namespace Eiffel.Messaging.Tests.Mocks.Messages
{
    public class MockQuery : IQuery<MockQueryResult>
    {
    }

    public class MockQueryResult
    {
        public List<object> Items { get; set; }
    }

    public class MockUnknownQuery : IQuery<object>
    {

    }
}
