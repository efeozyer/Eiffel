using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eiffel.Persistence.MongoDB
{
    public class CollectionTypeMetadata<TCollection> : ICollectionTypeMetadata<TCollection>
       where TCollection : class
    {
        public string CollectionName { get; internal set; }
        public IEnumerable<TCollection> Data { get; internal set; }
        public Expression<Func<TCollection, bool>> FilterExpression { get; internal set; }
        public DocumentValidationAction ValidationAction { get; internal set; }
        public DocumentValidationLevel ValidationLevel { get; internal set; }
        public bool? IsCapped { get; internal set; }
        public long? MaxSize { get; internal set; }
        public long? MaxDocuments { get; internal set; }
        public CreateCollectionOptions<TCollection> CollectionOptions { get; internal set; }
        public MongoCollectionSettings ColletionSettings { get; internal set; }
    }
}
