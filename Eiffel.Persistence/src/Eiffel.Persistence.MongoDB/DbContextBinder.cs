using Eiffel.Persistence.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eiffel.Persistence.MongoDb
{
    public static class DbContextBinder<TContext>
        where TContext : DbContext
    {
        public static void Bind(TContext context, IServiceProvider serviceProvider)
        {
            foreach (var property in GetProperties(context))
            {
                var config = GetCollectionConfig(property, serviceProvider);
                var collection = GetCollection(context, config.Name, property.PropertyType.GetGenericArguments()[0], config.CollectionSettings);
                if (collection == null)
                {
                    context.Database.CreateCollection(config.Name);
                    collection = context.Database.GetCollection(config.Name);
                }

                property.SetValue(context, CreateCollectionSet(property, collection));
            }
        }

        private static IEnumerable<PropertyInfo> GetProperties(TContext context)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach(var property in context.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType))
            {
                if (property.PropertyType == typeof(DbSet<>).MakeGenericType(property.PropertyType.GetGenericArguments())) {
                    properties.Add(property);
                }
            }
            return properties;
        }

        private static dynamic GetCollectionConfig(PropertyInfo property, IServiceProvider serviceProvider)
        {
            var configType = typeof(ICollectionTypeConfiguration<>).MakeGenericType(property.PropertyType.GetGenericArguments());
            var collectionConfig = (dynamic)serviceProvider.GetService(configType);
            if (collectionConfig == null)
            {
                throw new CollectionBindingException($"{property.Name} CollectionTypeConfiguration could not be found.");
            }
            return collectionConfig;
        }

        private static object CreateCollectionSet(PropertyInfo property, object mongoCollection)
        {
            var collectionType = typeof(DbSet<>).MakeGenericType(property.PropertyType.GetGenericArguments());
            return Activator.CreateInstance(collectionType, new[] { mongoCollection });
        }

        private static object GetCollection(TContext context, string collectionName, Type collectionType, MongoCollectionSettings collectionSettings = null)
        {
            var getCollectionMethod = context.Database.GetType().GetMethod("GetCollection")
                .MakeGenericMethod(new[] { collectionType });

            return (dynamic)getCollectionMethod.Invoke(context.Database, new object[] { collectionName, collectionSettings });
        }
    }
}
