using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public interface ICollectionTypeMetadata<TCollection>
        where TCollection : class
    {
        string CollectionName { get; }
        IEnumerable<TCollection> Data { get; }
        Expression<Func<TCollection, bool>> FilterExpression { get; }
        DocumentValidationAction ValidationAction { get; }
        DocumentValidationLevel ValidationLevel { get; }
        bool? IsCapped { get; }
        long? MaxSize { get; }
        long? MaxDocuments { get; }
        CreateCollectionOptions<TCollection> CollectionOptions { get; }
        MongoCollectionSettings ColletionSettings { get; }
    }
}
