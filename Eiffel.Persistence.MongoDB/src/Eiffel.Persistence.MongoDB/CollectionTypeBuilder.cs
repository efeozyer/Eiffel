using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Eiffel.Persistence.MongoDB
{
    public class CollectionTypeBuilder<TEntity> : ICollectionTypeBuilder<TEntity>
        where TEntity : class
    {
        private readonly CollectionTypeMetadata<TEntity> _metadata;
        private readonly List<FilterDefinition<TEntity>> _validationRules;
        private readonly List<IndexKeysDefinition<TEntity>> _indexKeys;

        public CollectionTypeBuilder()
        {
            _metadata = new CollectionTypeMetadata<TEntity>();
            _validationRules = new List<FilterDefinition<TEntity>>();
            _indexKeys = new List<IndexKeysDefinition<TEntity>>();
        }

        public ICollectionTypeBuilder<TEntity> ToCollection(string name)
        {
            _metadata.CollectionName = name;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> HasQueryFilter(Expression<Func<TEntity, bool>> expression)
        {
            _metadata.FilterExpression = _metadata.FilterExpression == null ? expression : _metadata.FilterExpression.CombineWith(expression);
            return this;
        }

        // TODO: Implement all index types
        public ICollectionTypeBuilder<TEntity> HasIndex(Expression<Func<TEntity, object>> expression, bool isAscending = true)
        {
            if (isAscending)
            {
                _indexKeys.Add(Builders<TEntity>.IndexKeys.Ascending(expression));
            }
            else
            {
                _indexKeys.Add(Builders<TEntity>.IndexKeys.Descending(expression));
            }
            return this;
        }

        public ICollectionTypeBuilder<TEntity> HasData(IEnumerable<TEntity> data)
        {
            _metadata.Data = data;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> IsRequired(Expression<Func<TEntity, object>> expression, bool isRequired = true)
        {
            _validationRules.Add(Builders<TEntity>.Filter.Exists(expression, isRequired));
            return this;
        }

        public ICollectionTypeBuilder<TEntity> Type(Expression<Func<TEntity, object>> expression, BsonType valueType)
        {
            _validationRules.Add(Builders<TEntity>.Filter.Type(expression, valueType));
            return this;
        }

        public ICollectionTypeBuilder<TEntity> ValidationLevel(DocumentValidationLevel validationLevel)
        {
            _metadata.ValidationLevel = validationLevel;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> ValidationAction(DocumentValidationAction validationAction)
        {
            _metadata.ValidationAction = validationAction;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> UsePowerOf2Sizes(bool usePowerOf2Sizes)
        {
            _metadata.CollectionOptions.UsePowerOf2Sizes = usePowerOf2Sizes;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> IsCapped(bool isCapped, long? maxSize, long? maxDocuments)
        {
            _metadata.IsCapped = isCapped;
            _metadata.MaxSize = maxSize;
            _metadata.MaxDocuments = maxDocuments;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> Collation(Collation collation)
        {
            _metadata.CollectionOptions.Collation = collation;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> NoPadding(bool noPadding)
        {
            _metadata.CollectionOptions.NoPadding = noPadding;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> StorageEngine(BsonDocument storageEngine)
        {
            _metadata.CollectionOptions.StorageEngine = storageEngine;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> AssignIdOnInsert(bool assignIdOnInsert)
        {
            _metadata.ColletionSettings.AssignIdOnInsert = assignIdOnInsert;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> ReadConcern(ReadConcern readConcern)
        {
            _metadata.ColletionSettings.ReadConcern = readConcern;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> ReadEncoding(UTF8Encoding readEncoding)
        {
            _metadata.ColletionSettings.ReadEncoding = readEncoding;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> ReadPreference(ReadPreference readPreference)
        {
            _metadata.ColletionSettings.ReadPreference = readPreference;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> WriteConcern(WriteConcern writeConcern)
        {
            _metadata.ColletionSettings.WriteConcern = writeConcern;
            return this;
        }

        public ICollectionTypeBuilder<TEntity> WriteEncoding(UTF8Encoding writeEnconding)
        {
            _metadata.ColletionSettings.WriteEncoding = writeEnconding;
            return this;
        }

        // TODO : Create index options
        public CollectionTypeMetadata<TEntity> Build()
        {
            if (_metadata.IsCapped.HasValue && _metadata.IsCapped.Value)
            {
                _metadata.CollectionOptions.MaxDocuments = _metadata.MaxDocuments;
                _metadata.CollectionOptions.MaxSize = _metadata.MaxSize;
            }

            if (_validationRules?.Count > 0)
            {
                _metadata.CollectionOptions.Validator = new FilterDefinitionBuilder<TEntity>().And(_validationRules);
                _metadata.CollectionOptions.ValidationAction = _metadata.ValidationAction;
                _metadata.CollectionOptions.ValidationLevel = _metadata.ValidationLevel;
            }

            return _metadata;
        }
    }

    internal static class ExpressionExtensions
    {
        internal static Expression<Func<T, bool>> CombineWith<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression parameter = left.Parameters[0];
            var visitor = new ReplaceParameterVisitor(right.Parameters[0], parameter);
            var body = visitor.Visit(right.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, body), parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _left;
            private readonly ParameterExpression _right;

            public ReplaceParameterVisitor(ParameterExpression left, ParameterExpression right)
            {
                _left = left;
                _right = right;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ReferenceEquals(node, _left))
                    return _right;

                return base.VisitParameter(node);
            }
        }
    }
}
