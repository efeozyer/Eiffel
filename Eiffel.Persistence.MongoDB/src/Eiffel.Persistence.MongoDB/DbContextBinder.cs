using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Eiffel.Persistence.MongoDB
{
    public static class DbContextBinder<TContext>
        where TContext : DbContext
    {
        public static void Bind(TContext context, IServiceProvider serviceProvider)
        {
            var existingCollections = context.Database.ListCollections().ToListAsync().Result;
            foreach(var property in GetDbContextProperties(context))
            {
                var metadata = GetCollectionMetadata(property, serviceProvider);
                if (!existingCollections.Select(x => x["name"].AsString).Contains(metadata.CollectionName))
                {
                    context.Database.CreateCollection(metadata.CollectionName, metadata.CollectionOptions);
                }

                var collection = GetCollection(context, metadata.CollectionName, property.PropertyType, metadata.ColletionSettings);
                if (metadata?.IndexKeys?.Count > 0)
                {
                    EnsureIndexes(property.PropertyType, collection, metadata.IndexKeys);
                }

                var contextCollection = CreateCollection(property.PropertyType, collection, metadata.FilterExpression);
                if (metadata.Documents?.Count > 0)
                {
                    SeedCollection(contextCollection, metadata.Documents);
                }

                property.SetValue(context, contextCollection);
            }
        }

        private static void EnsureIndexes(Type collectionType, dynamic collection, dynamic indexKeys)
        {
            foreach(var indexKey in indexKeys)
            {
                CreateIndex(collectionType, collection, indexKey);
            }
        }

        private static IEnumerable<PropertyInfo> GetDbContextProperties(TContext context)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach (var property in context.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType))
            {
                if (property.PropertyType == typeof(Collection<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]))
                {
                    properties.Add(property);
                }
            }
            return properties;
        }

        private static CollectionTypeMetadata GetCollectionMetadata(PropertyInfo propertyInfo, IServiceProvider serviceProvider)
        {
            var collectionTypeConfig = GetCollectionConfiguration(propertyInfo.PropertyType, serviceProvider);
            var collectionTypeBuilder = CreateCollectionTypeBuilder(propertyInfo.PropertyType);
            collectionTypeConfig.Configure(collectionTypeBuilder);
            return (CollectionTypeMetadata)collectionTypeBuilder.Build();
        }

        private static dynamic GetCollectionConfiguration(Type propertyType, IServiceProvider serviceProvider)
        {
            var instanceType = typeof(ICollectionTypeConfiguration<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            return serviceProvider.GetService(instanceType);
        }

        private static dynamic CreateCollectionTypeBuilder(Type propertyType)
        {
            var instanceType = typeof(CollectionTypeBuilder<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            return Activator.CreateInstance(instanceType);
        }

        private static object CreateCollection(Type collectionType, object collection, dynamic filterExpression)
        {
            var instanceType = typeof(Collection<>).MakeGenericType(collectionType.GetGenericArguments()[0]);
            return Activator.CreateInstance(instanceType, new[] { collection, filterExpression });
        }

        //
        // TODO: Move proxy methods to antoher class
        //
        private static dynamic GetCollection(TContext context, string collectionName, Type collectionType, MongoCollectionSettings collectionSettings = null)
        {
            var genericMethod = context.Database.GetType().GetMethod("GetCollection")
                .MakeGenericMethod(collectionType.GetGenericArguments()[0]);

            return (dynamic)genericMethod.Invoke(context.Database, new object[] { collectionName, collectionSettings });
        }

        private static void CreateIndex(Type collectionType, dynamic collection, dynamic indexDefinition)
        {
            var instanceType = typeof(CreateIndexModel<>).MakeGenericType(collectionType.GetGenericArguments()[0]);
            var instance = Activator.CreateInstance(instanceType, new[] { indexDefinition, null });

            var createIndexMethod = collection.Indexes.GetType().GetMethod("CreateOne", new[] { instanceType, typeof(CreateOneIndexOptions), typeof(CancellationToken) });
            createIndexMethod.Invoke(collection.Indexes, new[] { instance, null, default });
        }

        private static void SeedCollection(dynamic collection, dynamic documents)
        {
            var genericMethod = collection.GetType().GetMethod("AddRange");
            genericMethod.Invoke(collection, new[] { documents });
        }
    }
}
