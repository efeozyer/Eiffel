using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Eiffel.Persistence.MongoDb
{
    public class DbSet<TDocument> : IQueryable<TDocument>
        where TDocument : class
    {
        private readonly IMongoCollection<TDocument> _collection;

        public DbSet(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public virtual IAsyncEnumerable<TDocument> AsAsyncEnumerable()
        {
            return (IAsyncEnumerable<TDocument>)_collection;
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual Type ElementType
        {
            get
            {
                return typeof(TDocument);
            }
        }

        public virtual Expression Expression
        {
            get
            {
                return _collection.AsQueryable().Expression;
            }
        }

        public virtual IQueryProvider Provider
        {
            get
            {
                return _collection.AsQueryable().Provider;
            }
        }

        public virtual IEnumerator<TDocument> GetEnumerator()
        {
            return _collection.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
