using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Eiffel.Messaging.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, Action<MiddlewareOptions> options = null)
        {
            MiddlewareOptions middlewareOptions = new MiddlewareOptions();
            options?.Invoke(middlewareOptions);
            
            services.AddSingleton<IMediator>(serviceProvider =>
            {
                return new Mediator(serviceProvider, middlewareOptions);
            });

            services.AddMessageHandlers();
            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.RegisterType(typeof(ICommandHandler<>));
            services.RegisterType(typeof(IQueryHandler<,>));
            services.RegisterType(typeof(IEventHandler<>));
            return services;
        }

        public static IServiceCollection AddMessageBus<TClient, TConfig>(this IServiceCollection services, Action<MiddlewareOptions> options = null)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            MiddlewareOptions middlewareOptions = new MiddlewareOptions();
            options?.Invoke(middlewareOptions);

            services.AddSingleton(serviceProvider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                return config.Bind(serviceProvider.GetRequiredService<IConfiguration>());
            });

            services.AddSingleton(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<TClient>>();
                var config = serviceProvider.GetService<IMessageQueueClientConfig>();
                return (IMessageQueueClient)Activator.CreateInstance(typeof(IMessageQueueClient), new object [] { logger, config });
            });

            services.AddSingleton(serviceProvider =>
            {
                var client = (IMessageQueueClient)serviceProvider.GetRequiredService(typeof(TClient));
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                return new MessageBus(client, mediator, middlewareOptions);
            });
            return services;
        }

        public static IServiceCollection AddEventBus<TClient, TConfig>(this IServiceCollection services)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            services.AddSingleton(serviceProvider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                return config.Bind(serviceProvider.GetRequiredService<IConfiguration>());
            });

            services.AddSingleton(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<TClient>>();
                var config = serviceProvider.GetService<IMessageQueueClientConfig>();
                return (IMessageQueueClient)Activator.CreateInstance(typeof(IMessageQueueClient), new object[] { logger, config });
            });

            services.AddSingleton(serviceProvider =>
            {
                var client = (IMessageQueueClient)serviceProvider.GetRequiredService(typeof(TClient));
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                return new EventBus(client, mediator);
            });
            return services;
        }

        private static void RegisterType(this IServiceCollection services, Type targetType)
        {
            services.Scan(x => x.FromApplicationDependencies()
                .AddClasses(s => s.AssignableTo(targetType).Where(f => !f.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
