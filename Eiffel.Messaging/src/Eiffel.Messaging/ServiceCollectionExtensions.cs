using Autofac;
using Eiffel.Messaging.Abstractions;
using Eiffel.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Eiffel.Messaging
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddMediator(this ContainerBuilder builder, Assembly[] assemblies = null)
        {
            if (assemblies == null)
            {
                assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(Assembly.LoadFrom)
                    .ToArray();
            }

            builder.AddSingleton<IMediator, Mediator>();
            builder.RegisterHandlers(assemblies);
            builder.RegisterPipelines(assemblies);

            return builder;
        }

        public static ContainerBuilder AddSingleton<TService, TImplementation>(this ContainerBuilder builder)
        {
            builder.RegisterType<TImplementation>().As<TService>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddMessageBus<TClient, TConfig>(this ContainerBuilder builder)
           where TClient : class, IMessageQueueClient
           where TConfig : class, IMessageQueueClientConfig
        {
            builder.Register(context =>
            {
                var config = Activator.CreateInstance<TConfig>();
                config.Bind(context.Resolve<IConfiguration>());

                var logger = context.Resolve<ILogger<TClient>>();
                var client = (IMessageQueueClient)Activator.CreateInstance(typeof(TClient), new object[] { logger, config });

                var mediator = context.Resolve<IMediator>();

                return new MessageBus(client, mediator);
            }).SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddEventBus<TClient, TConfig>(this ContainerBuilder builder)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            builder.Register<IEventBus>(context =>
            {
                var config = Activator.CreateInstance<TConfig>();
                config.Bind(context.Resolve<IConfiguration>());

                var logger = context.Resolve<ILogger<TClient>>();
                var client = (IMessageQueueClient)Activator.CreateInstance(typeof(TClient), new object[] { logger, config });

                var mediator = context.Resolve<IMediator>();

                return new EventBus(client, mediator);
            }).SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddMessaging<TClient, TConfig>(this ContainerBuilder builder)
            where TClient : class, IMessageQueueClient
            where TConfig : class, IMessageQueueClientConfig
        {
            builder.AddMediator();
            builder.AddEventBus<TClient, TConfig>();
            builder.AddMessageBus<TClient, TConfig>();

            return builder;
        }

        private static ContainerBuilder RegisterHandlers(this ContainerBuilder builder, Assembly[] assemblies = null)
        {
           builder.RegisterAssemblyTypes(assemblies)
               .AsClosedTypesOf(typeof(CommandHandler<>))
               .AsSelf()
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies)
               .AsClosedTypesOf(typeof(QueryHandler<,>))
               .AsSelf()
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies)
               .AsClosedTypesOf(typeof(Abstractions.EventHandler<>))
               .AsSelf()
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies)
               .AsClosedTypesOf(typeof(MessageHandler<>))
               .AsSelf()
               .InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterPipelines(this ContainerBuilder builder, Assembly[] assemblies = null)
        {
            builder.RegisterAssemblyTypes(assemblies)
               .AssignableTo(typeof(IPipelinePreProcessor))
               .AsSelf()
               .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
              .AssignableTo(typeof(IPipelinePostProcessor))
              .AsSelf()
              .SingleInstance();

            return builder;
        }
    }
}
