using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Eiffel.Messaging.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, Action<MiddlewareOptions> options = null)
        {
            MiddlewareOptions middlewareOptions = default;
            if (options != null)
            {
                middlewareOptions = new MiddlewareOptions();
                options.Invoke(middlewareOptions);
            }
            
            services.AddSingleton<IMediator, Mediator>(x =>
            {
                return new Mediator(x, middlewareOptions?.GetMiddlewares() ?? new List<IMessagingMiddleware>());
            });

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

        public static IServiceCollection AddMessageBus(this IServiceCollection services, Action<MiddlewareOptions> options = null)
        {
            MiddlewareOptions middlewareOptions = default;
            if (options != null)
            {
                middlewareOptions = new MiddlewareOptions();
                options.Invoke(middlewareOptions);
            }

            // TODO: Register message bus
            return services;
        }

        public static IServiceCollection AddEventBus<TClient, TConfig>(this IServiceCollection services, IConfiguration config)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageClientConfig
        {
            var clientConfig = Activator.CreateInstance<TConfig>();
            services.AddSingleton(clientConfig.Bind(config));
            services.AddSingleton<IMessageQueueClient, TClient>();
            services.AddSingleton<IEventBus, EventBus>();
            return services;
        }

        public static IServiceCollection AddEventBus<TClient>(this IServiceCollection services, TClient client)
            where TClient : class, IMessageQueueClient
        {
            services.AddSingleton<IMessageQueueClient>(client);
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
