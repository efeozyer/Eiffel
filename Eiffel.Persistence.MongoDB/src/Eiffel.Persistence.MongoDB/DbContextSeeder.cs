using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.MongoDB.Collections;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Eiffel.Persistence.MongoDB
{
    public static class DbContextSeeder<TContext>
        where TContext : DbContext
    {
        internal const string SeedHistoryCollectionName = "__SeedHistory";
        public static void Seed(TContext context, IServiceProvider serviceProvider)
        {
            var existingCollections = GenericMethodProxy<TContext>.GetExistingCollections(context);
            if (!existingCollections.Contains(SeedHistoryCollectionName))
            {
                context.Database.CreateCollection(SeedHistoryCollectionName);
            }

            // TODO: Start & Commit transaction
            foreach (var property in context.GetProperties())
            {
                var metadata = property.GetCollectionMetadata(serviceProvider);
                if (metadata.Documents?.Count > 0 && !IsCollectionAlreadySeeded(context, metadata.CollectionName))
                {
                    var collection = GenericMethodProxy<TContext>.GetCollection(context, metadata.CollectionName, property.PropertyType, metadata.ColletionSettings);
                    var contextProperty = GenericMethodProxy <TContext>.CreateCollection(property.PropertyType, collection, metadata.FilterExpression);
                    var isSeedingSucceeded = GenericMethodProxy<TContext>.SeedCollection(contextProperty, metadata.Documents);
                    if (isSeedingSucceeded)
                    {
                        AddToSeedHistory(context, metadata.CollectionName, context.Database.DatabaseNamespace.DatabaseName, metadata.Documents.Count);
                    }
                }
            }
        }

        private static bool IsCollectionAlreadySeeded(TContext context, string collectionName)
        {
            return GetHistoryCollection(context).Find(x => x.CollectionName == collectionName).Any();
        }

        private static void AddToSeedHistory(TContext context, string collectionName, string databaseName, long documentCount)
        {
            GetHistoryCollection(context)
                .InsertOne(new SeedHistory
            {
                CollectionName = collectionName,
                Database = databaseName,
                DocumentCount = documentCount,
                CreatedOn = DateTime.UtcNow
            });
        }

        private static IMongoCollection<SeedHistory> GetHistoryCollection(TContext context)
        {
            return context.Database.GetCollection<SeedHistory>(SeedHistoryCollectionName);
        }
    }
}
