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
        public static IServiceCollection AddMediator(this IServiceCollection services, Action<MessagingMiddlewareOptions> options = null)
        {
            MessagingMiddlewareOptions middlewareOptions = new MessagingMiddlewareOptions();
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

        public static IServiceCollection AddMessageBus<TClient, TConfig>(this IServiceCollection services, Action<MessagingMiddlewareOptions> options = null)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            MessagingMiddlewareOptions middlewareOptions = new MessagingMiddlewareOptions();
            options?.Invoke(middlewareOptions);

            services.AddSingleton(serviceProvider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                config.Bind(serviceProvider.GetRequiredService<IConfiguration>());

                var logger = serviceProvider.GetService<ILogger<TClient>>();
                var client = (IMessageQueueClient)Activator.CreateInstance(typeof(TClient), new object[] { logger, config });

                var mediator = serviceProvider.GetRequiredService<IMediator>();
                return new MessageBus(client, mediator, middlewareOptions);
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

        public static IServiceCollection AddMessaging<TClient, TConfig>(this IServiceCollection services, Action<MessagingMiddlewareOptions> options = null)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            services.AddMessageBus<TClient, TConfig>(options);
            services.AddEventBus<TClient, TConfig>();
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
