using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;
using System;
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

        public static void SeedCollection(dynamic collection, dynamic documents)
        {
            var genericMethod = collection.GetType().GetMethod("AddRange");
            genericMethod.Invoke(collection, new[] { documents });
        }
    }
}
