using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Eiffel.Persistence.MongoDB
{
    public class CollectionTypeBuilder<TCollection> : ICollectionTypeBuilder<TCollection>
        where TCollection : class
    {
        private readonly CollectionTypeMetadata _metadata;
        private readonly List<FilterDefinition<TCollection>> _validationRules;
        private readonly List<IndexKeysDefinition<TCollection>> _indexKeys;

        public CollectionTypeBuilder()
        {
            _metadata = new CollectionTypeMetadata();
            _validationRules = new List<FilterDefinition<TCollection>>();
            _indexKeys = new List<IndexKeysDefinition<TCollection>>();
        }

        public ICollectionTypeBuilder<TCollection> ToCollection(string name)
        {
            _metadata.CollectionName = name;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> HasQueryFilter(Expression<Func<TCollection, bool>> expression)
        {
            _metadata.FilterExpression = _metadata.FilterExpression == null ? (dynamic)expression : _metadata.FilterExpression.CombineWith(expression);
            return this;
        }

        // TODO: Implement all index types
        public ICollectionTypeBuilder<TCollection> HasIndex(Expression<Func<TCollection, object>> expression, bool isAscending = true)
        {
            if (isAscending)
            {
                _indexKeys.Add(Builders<TCollection>.IndexKeys.Ascending(expression));
            }
            else
            {
                _indexKeys.Add(Builders<TCollection>.IndexKeys.Descending(expression));
            }
            return this;
        }

        public ICollectionTypeBuilder<TCollection> HasDocuments(IEnumerable<TCollection> documents)
        {
            _metadata.Documents = documents;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> IsRequired(Expression<Func<TCollection, object>> expression, bool isRequired = true)
        {
            _validationRules.Add(Builders<TCollection>.Filter.Exists(expression, isRequired));
            return this;
        }

        public ICollectionTypeBuilder<TCollection> Type(Expression<Func<TCollection, object>> expression, BsonType valueType)
        {
            _validationRules.Add(Builders<TCollection>.Filter.Type(expression, valueType));
            return this;
        }

        public ICollectionTypeBuilder<TCollection> ValidationLevel(DocumentValidationLevel validationLevel)
        {
            _metadata.ValidationLevel = validationLevel;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> ValidationAction(DocumentValidationAction validationAction)
        {
            _metadata.ValidationAction = validationAction;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> UsePowerOf2Sizes(bool usePowerOf2Sizes)
        {
            _metadata.CollectionOptions.UsePowerOf2Sizes = usePowerOf2Sizes;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> IsCapped(bool isCapped, long? maxSize, long? maxDocuments)
        {
            _metadata.IsCapped = isCapped;
            _metadata.MaxSize = maxSize;
            _metadata.MaxDocuments = maxDocuments;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> Collation(Collation collation)
        {
            _metadata.CollectionOptions.Collation = collation;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> NoPadding(bool noPadding)
        {
            _metadata.CollectionOptions.NoPadding = noPadding;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> StorageEngine(BsonDocument storageEngine)
        {
            _metadata.CollectionOptions.StorageEngine = storageEngine;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> AssignIdOnInsert(bool assignIdOnInsert)
        {
            _metadata.ColletionSettings.AssignIdOnInsert = assignIdOnInsert;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> ReadConcern(ReadConcern readConcern)
        {
            _metadata.ColletionSettings.ReadConcern = readConcern;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> ReadEncoding(UTF8Encoding readEncoding)
        {
            _metadata.ColletionSettings.ReadEncoding = readEncoding;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> ReadPreference(ReadPreference readPreference)
        {
            _metadata.ColletionSettings.ReadPreference = readPreference;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> WriteConcern(WriteConcern writeConcern)
        {
            _metadata.ColletionSettings.WriteConcern = writeConcern;
            return this;
        }

        public ICollectionTypeBuilder<TCollection> WriteEncoding(UTF8Encoding writeEnconding)
        {
            _metadata.ColletionSettings.WriteEncoding = writeEnconding;
            return this;
        }

        // TODO : Create index options
        public ICollectionTypeMetadata Build()
        {
            _metadata.CollectionOptions = new CreateCollectionOptions<TCollection>();
            if (_indexKeys?.Count > 0)
            {
                _metadata.IndexKeys = _indexKeys;
            }

            if (_metadata.IsCapped.HasValue && _metadata.IsCapped.Value)
            {
                _metadata.CollectionOptions.MaxDocuments = _metadata.MaxDocuments;
                _metadata.CollectionOptions.MaxSize = _metadata.MaxSize;
            }

            if (_validationRules?.Count > 0)
            {
                _metadata.CollectionOptions.Validator =  new FilterDefinitionBuilder<TCollection>().And(_validationRules.Select(x => x));
                _metadata.CollectionOptions.ValidationAction = _metadata.ValidationAction ?? _metadata.CollectionOptions.ValidationAction;
                _metadata.CollectionOptions.ValidationLevel = _metadata.ValidationLevel ?? _metadata.CollectionOptions.ValidationLevel;
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
