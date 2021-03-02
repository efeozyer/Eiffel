using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eiffel.Persistence.MongoDB
{
    public class CollectionTypeMetadata<TEntity>
       where TEntity : class
    {
        public string CollectionName { get; set; }
        public IEnumerable<TEntity> Data { get; internal set; }
        public Expression<Func<TEntity, bool>> FilterExpression { get; internal set; }
        public DocumentValidationAction ValidationAction { get; internal set; }
        public DocumentValidationLevel ValidationLevel { get; internal set; }
        public bool? IsCapped { get; internal set; }
        public long? MaxSize { get; internal set; }
        public long? MaxDocuments { get; internal set; }
        internal CreateCollectionOptions<TEntity> CollectionOptions { get; set; }
        internal MongoCollectionSettings ColletionSettings { get; set; }
    }
}
