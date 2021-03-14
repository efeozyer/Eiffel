using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Eiffel.Persistence.MongoDB
{
    public static class GenericMethodProxy<TContext>
        where TContext : DbContext
    {
        public static dynamic GetCollection(TContext context, string collectionName, Type collectionType, MongoCollectionSettings collectionSettings = null)
        {
            var genericMethod = context.Database.GetType().GetMethod("GetCollection")
                .MakeGenericMethod(collectionType.GetGenericArguments()[0]);

            return (dynamic)genericMethod.Invoke(context.Database, new object[] { collectionName, collectionSettings });
        }

        public static void CreateIndex(Type collectionType, dynamic collection, dynamic indexDefinition)
        {
            var instanceType = typeof(CreateIndexModel<>).MakeGenericType(collectionType.GetGenericArguments()[0]);
            var instance = Activator.CreateInstance(instanceType, new[] { indexDefinition, null });

            var createIndexMethod = collection.Indexes.GetType().GetMethod("CreateOne", new[] { instanceType, typeof(CreateOneIndexOptions), typeof(CancellationToken) });
            createIndexMethod.Invoke(collection.Indexes, new[] { instance, null, default });
        }

        public static bool SeedCollection(dynamic collection, dynamic documents)
        {
            try
            {
                var genericMethod = collection.GetType().GetMethod("AddRange");
                genericMethod.Invoke(collection, new[] { documents });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SeedCollection(IClientSessionHandle sessionHandle, dynamic collection, dynamic documents)
        {
            try
            {
                var genericMethod = collection.GetType().GetMethod("AddRange");
                genericMethod.Invoke(collection, new[] { sessionHandle, documents });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static List<string> GetExistingCollections(TContext context)
        {
            var existingCollections = context.Database.ListCollections().ToListAsync().Result;
            return existingCollections.Select(x => x["name"].AsString).ToList();
        }

        internal static object CreateCollection(Type propertyType, object collection, dynamic filterExpression)
        {
            var instanceType = typeof(Collection<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            return Activator.CreateInstance(instanceType, new[] { collection, filterExpression });
        }
    }
}
