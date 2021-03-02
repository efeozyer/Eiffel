﻿using Eiffel.Persistence.MongoDB.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eiffel.Persistence.MongoDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDocumentContext<TContext>(this IServiceCollection services, DbContextOptions<TContext> options)
            where TContext : DbContext
        {
            services.Scan(x => x.FromApplicationDependencies()
                   .AddClasses(s => s.AssignableTo(typeof(ICollectionTypeConfiguration<>)).Where(f => !f.IsGenericType))
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime());

            services.AddTransient(serviceProvider =>
            {
                var context = (TContext)Activator.CreateInstance(typeof(TContext), new object[] { options });
                DbContextBinder<TContext>.Bind(context, serviceProvider);
                return context;
            });

            return services;
        }
    }
}
