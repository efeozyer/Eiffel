using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Eiffel.Persistence.MongoDB.Abstractions
{
    public interface ICollectionTypeBuilder<TEntity>
    {
        ICollectionTypeBuilder<TEntity> ToCollection(string name);
        ICollectionTypeBuilder<TEntity> HasQueryFilter(Expression<Func<TEntity, bool>> expression);
        ICollectionTypeBuilder<TEntity> HasIndex(Expression<Func<TEntity, object>> expression, bool isAscending = true);
        ICollectionTypeBuilder<TEntity> HasData(IEnumerable<TEntity> data);
        ICollectionTypeBuilder<TEntity> IsRequired(Expression<Func<TEntity, object>> expression, bool isRequired = true);
        ICollectionTypeBuilder<TEntity> Type(Expression<Func<TEntity, object>> expression, BsonType valueType);
        ICollectionTypeBuilder<TEntity> ValidationLevel(DocumentValidationLevel validationLevel);
        ICollectionTypeBuilder<TEntity> ValidationAction(DocumentValidationAction validationAction);
        ICollectionTypeBuilder<TEntity> UsePowerOf2Sizes(bool usePowerOf2Sizes);
        ICollectionTypeBuilder<TEntity> IsCapped(bool isCapped, long? maxSize, long? maxDocuments);
        ICollectionTypeBuilder<TEntity> Collation(Collation collation);
        ICollectionTypeBuilder<TEntity> NoPadding(bool noPadding);
        ICollectionTypeBuilder<TEntity> StorageEngine(BsonDocument storageEngine);
        ICollectionTypeBuilder<TEntity> AssignIdOnInsert(bool assignIdOnInsert);
        ICollectionTypeBuilder<TEntity> ReadConcern(ReadConcern readConcern);
        ICollectionTypeBuilder<TEntity> ReadEncoding(UTF8Encoding readEncoding);
        ICollectionTypeBuilder<TEntity> ReadPreference(ReadPreference readPreference);
        ICollectionTypeBuilder<TEntity> WriteConcern(WriteConcern writeConcern);
        ICollectionTypeBuilder<TEntity> WriteEncoding(UTF8Encoding writeEnconding);
    }
}
