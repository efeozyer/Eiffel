using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockAsyncCursor<TDocument> : IAsyncCursor<TDocument>
        where TDocument : class
    {
        private readonly List<TDocument> _documents = new List<TDocument>();
        public IEnumerable<TDocument> Current => _documents;

        public MockAsyncCursor(IEnumerable<TDocument> documents)
        {
            AddRange(documents);
        }

        public void Dispose()
        {
        }

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            return true;
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public void Add(TDocument document)
        {
            _documents.Add(document);
        }

        public void AddRange(IEnumerable<TDocument> documents)
        {
            _documents.AddRange(documents);
        }
    }
}
