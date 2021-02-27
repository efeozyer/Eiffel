using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eiffel.Persistence.MongoDb.Tests.Mocks
{
    public class MockAsyncCursor<T> : IAsyncCursor<T>
        where T : class
    {
        private readonly List<T> _items = new List<T>();

        public IEnumerable<T> Current => _items;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            return true;
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public void Add(T item)
        {
            _items.Add(item);
        }
    }
}
