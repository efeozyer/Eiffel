using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Eiffel.Messaging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddSingleton<IMediator>(serviceProvider =>
            {
                return new Mediator(serviceProvider);
            });

            services.AddMessageHandlers();
            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.RegisterType(typeof(CommandHandler<>));
            services.RegisterType(typeof(QueryHandler<,>));
            services.RegisterType(typeof(Abstractions.EventHandler<>));
            services.RegisterType(typeof(MessageHandler<>));
            return services;
        }

        public static IServiceCollection AddMessageBus<TClient, TConfig>(this IServiceCollection services)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            services.AddSingleton(serviceProvider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                config.Bind(serviceProvider.GetRequiredService<IConfiguration>());

                var logger = serviceProvider.GetService<ILogger<TClient>>();
                var client = (IMessageQueueClient)Activator.CreateInstance(typeof(TClient), new object[] { logger, config });

                var mediator = serviceProvider.GetRequiredService<IMediator>();
                return new MessageBus(client, mediator);
            });
            return services;
        }

        public static IServiceCollection AddEventBus<TClient, TConfig>(this IServiceCollection services)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            services.AddSingleton<IEventBus>(serviceProvider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                config.Bind(serviceProvider.GetRequiredService<IConfiguration>());

                var logger = serviceProvider.GetService<ILogger<TClient>>();
                var client = (IMessageQueueClient)Activator.CreateInstance(typeof(TClient), new object[] { logger, config });

                var mediator = serviceProvider.GetRequiredService<IMediator>();
                return new EventBus(client, mediator);
            });
            return services;
        }

        public static IServiceCollection AddMessaging<TClient, TConfig>(this IServiceCollection services)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            services.AddEventBus<TClient, TConfig>();
            return services;
        }

        private static void RegisterType(this IServiceCollection services, Type targetType)
        {
            services.Scan(x => x.FromApplicationDependencies()
                .AddClasses(x => x.AssignableTo(targetType).Where(x => !x.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
