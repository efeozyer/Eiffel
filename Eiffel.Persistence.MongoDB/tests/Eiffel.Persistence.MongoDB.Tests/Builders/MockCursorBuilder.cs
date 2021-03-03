using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;

namespace Eiffel.Persistence.MongoDB.Tests
{
    public static class MockCursorBuilder<TDocument>
    {
        public static IAsyncCursor<TDocument> Build(IEnumerable<TDocument> documents)
        {
            var mockCursor = new Mock<IAsyncCursor<TDocument>>() { CallBase = true };
            mockCursor.Setup(_ => _.Current).Returns(documents);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            return mockCursor.Object;
        }
    }
}
