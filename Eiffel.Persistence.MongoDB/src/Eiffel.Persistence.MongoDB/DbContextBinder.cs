using Eiffel.Persistence.MongoDB.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                if (!existingCollections.Select(x => x["name"].AsString).Contains((metadata.CollectionName as string)))
                {
                    context.Database.CreateCollection(metadata.CollectionName, metadata.CollectionOptions);
                }

                var collection = GetCollection(context, metadata.CollectionName, property.PropertyType, metadata.ColletionSettings);
                var contextCollection = CreateCollection(property.PropertyType, collection);
                property.SetValue(context, contextCollection);
            }
        }

        private static IEnumerable<PropertyInfo> GetDbContextProperties(TContext context)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach (var property in context.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType))
            {
                if (property.PropertyType == typeof(Collection<>).MakeGenericType(property.PropertyType.GetGenericArguments()))
                {
                    properties.Add(property);
                }
            }
            return properties;
        }

        private static dynamic GetCollectionMetadata(PropertyInfo propertyInfo, IServiceProvider serviceProvider)
        {
            var collectionTypeConfig = GetCollectionConfiguration(propertyInfo.PropertyType, serviceProvider);
            var collectionTypeBuilder = CreateCollectionTypeBuilder(propertyInfo.PropertyType);
            collectionTypeConfig.Configure(collectionTypeBuilder);
            return collectionTypeBuilder.Build();
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

        private static dynamic GetCollection(TContext context, string collectionName, Type collectionType, MongoCollectionSettings collectionSettings = null)
        {
            var getCollectionMethod = context.Database.GetType().GetMethod("GetCollection")
                .MakeGenericMethod(collectionType.GetGenericArguments()[0]);

            return (dynamic)getCollectionMethod.Invoke(context.Database, new object[] { collectionName, collectionSettings });
        }

        private static object CreateCollection(Type collectionType, object mongoCollection)
        {
            var instanceType = typeof(Collection<>).MakeGenericType(collectionType.GetGenericArguments()[0]);
            return Activator.CreateInstance(instanceType, new[] { mongoCollection });
        }
    }
}
