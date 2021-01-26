using System;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Abstractions.Event;
using Eiffel.Messaging.Abstractions.Query;
using Eiffel.Messaging.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;

namespace Eiffel.Messaging.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageDispatcher(this IServiceCollection services)
        {
            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
            return services;
        }

        private static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.RegisterType(typeof(ICommandHandler<>));
            services.RegisterType(typeof(IQueryHandler<,>));
            services.RegisterType(typeof(IEventHandler<>));
            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
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
