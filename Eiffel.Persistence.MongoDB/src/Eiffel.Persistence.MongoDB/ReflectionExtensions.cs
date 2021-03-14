using Eiffel.Persistence.MongoDB.Abstractions;
using Eiffel.Persistence.MongoDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eiffel.Persistence.MongoDB
{
    internal static class ReflectionExtensions
    {
        internal static IEnumerable<PropertyInfo> GetProperties<TContext>(this TContext context)
            where TContext : DbContext
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

        internal static dynamic GetCollectionConfiguration(this Type propertyType, IServiceProvider serviceProvider)
        {
            var instanceType = typeof(ICollectionTypeConfiguration<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            return serviceProvider.GetService(instanceType);
        }

        internal static CollectionTypeMetadata GetCollectionMetadata(this PropertyInfo propertyInfo, IServiceProvider serviceProvider)
        {
            var collectionTypeConfig = propertyInfo.PropertyType.GetCollectionConfiguration(serviceProvider);
            if (collectionTypeConfig == null)
            {
                throw new CollectionTypeConfigurationMissingException($"{propertyInfo.Name} collection type configuration is missing.");
            }
            var collectionTypeBuilder = propertyInfo.PropertyType.CreateCollectionTypeBuilder();
            collectionTypeConfig.Configure(collectionTypeBuilder);
            return (CollectionTypeMetadata)collectionTypeBuilder.Build();
        }

        internal static dynamic CreateCollectionTypeBuilder(this Type propertyType)
        {
            var instanceType = typeof(CollectionTypeBuilder<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            return Activator.CreateInstance(instanceType);
        }
    }
}
