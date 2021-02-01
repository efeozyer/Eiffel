using System;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Abstractions.Query;
using Eiffel.Messaging.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Eiffel.Messaging.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddSingleton<IMediator, Mediator>();
            services.AddMessageHandlers();
            services.AddMessagingMiddlewares();
            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.RegisterType(typeof(ICommandHandler<>));
            services.RegisterType(typeof(IQueryHandler<,>));
            services.RegisterType(typeof(IEventHandler<>));
            return services;
        }

        public static IServiceCollection AddMessagingMiddlewares(this IServiceCollection services)
        {
            services.RegisterType(typeof(IMessagingMiddleware));
            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddEventBus<TClient, TConfig>(this IServiceCollection services, IConfiguration config)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageClientConfig
        {
            var clientConfig = Activator.CreateInstance<TConfig>();
            clientConfig.Bind(config);
            services.AddSingleton(clientConfig);
            services.AddSingleton<IMessageQueueClient, TClient>();
            services.AddSingleton<IEventBus, EventBus>();
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
