using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.MongoDB.Exceptions;
using System;

namespace Eiffel.Persistence.MongoDB
{
    public static class DbContextBinder<TContext>
        where TContext : DbContext
    {
        public static void Bind(TContext context, IServiceProvider serviceProvider)
        {
            var existingCollections = GenericMethodProxy<TContext>.GetExistingCollections(context);
            foreach(var property in context.GetProperties())
            {
                var metadata = property.GetCollectionMetadata(serviceProvider);
                if (string.IsNullOrEmpty(metadata.CollectionName))
                {
                    throw new CollectionTypeConfigurationException("Collection name must be specified.");
                }

                if (!existingCollections.Contains(metadata.CollectionName))
                {
                    context.Database.CreateCollection(metadata.CollectionName, metadata.CollectionOptions);
                }

                var collection = GenericMethodProxy<TContext>.GetCollection(context, metadata.CollectionName, property.PropertyType, metadata.ColletionSettings);
                // TODO: Move another class, violates single responsibility principle
                if (metadata?.IndexKeys?.Count > 0)
                {
                    EnsureIndexes(property.PropertyType, collection, metadata.IndexKeys);
                }

                var contextCollection = GenericMethodProxy<TContext>.CreateCollection(property.PropertyType, collection, metadata.FilterExpression);
                property.SetValue(context, contextCollection);
            }
        }

        // TODO: Move another class, violates single responsibility principle
        private static void EnsureIndexes(Type collectionType, dynamic collection, dynamic indexKeys)
        {
            foreach(var indexKey in indexKeys)
            {
                GenericMethodProxy<TContext>.CreateIndex(collectionType, collection, indexKey);
            }
        }
    }
}
