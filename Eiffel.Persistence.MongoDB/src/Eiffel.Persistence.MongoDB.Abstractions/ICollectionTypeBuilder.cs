using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public interface ICollectionTypeBuilder<TCollection>
        where TCollection : class
    {
        ICollectionTypeBuilder<TCollection> ToCollection(string name);
        ICollectionTypeBuilder<TCollection> HasQueryFilter(Expression<Func<TCollection, bool>> expression);
        ICollectionTypeBuilder<TCollection> HasIndex(Expression<Func<TCollection, object>> expression, bool isAscending = true);
        ICollectionTypeBuilder<TCollection> HasDocuments(IEnumerable<TCollection> documents);
        ICollectionTypeBuilder<TCollection> IsRequired(Expression<Func<TCollection, object>> expression, bool isRequired = true);
        ICollectionTypeBuilder<TCollection> Type(Expression<Func<TCollection, object>> expression, BsonType valueType);
        ICollectionTypeBuilder<TCollection> ValidationLevel(DocumentValidationLevel validationLevel);
        ICollectionTypeBuilder<TCollection> ValidationAction(DocumentValidationAction validationAction);
        ICollectionTypeBuilder<TCollection> UsePowerOf2Sizes(bool usePowerOf2Sizes);
        ICollectionTypeBuilder<TCollection> IsCapped(bool isCapped, long? maxSize, long? maxDocuments);
        ICollectionTypeBuilder<TCollection> Collation(Collation collation);
        ICollectionTypeBuilder<TCollection> NoPadding(bool noPadding);
        ICollectionTypeBuilder<TCollection> StorageEngine(BsonDocument storageEngine);
        ICollectionTypeBuilder<TCollection> AssignIdOnInsert(bool assignIdOnInsert);
        ICollectionTypeBuilder<TCollection> ReadConcern(ReadConcern readConcern);
        ICollectionTypeBuilder<TCollection> ReadEncoding(UTF8Encoding readEncoding);
        ICollectionTypeBuilder<TCollection> ReadPreference(ReadPreference readPreference);
        ICollectionTypeBuilder<TCollection> WriteConcern(WriteConcern writeConcern);
        ICollectionTypeBuilder<TCollection> WriteEncoding(UTF8Encoding writeEnconding);
        ICollectionTypeMetadata Build();
    }
}
