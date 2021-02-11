using Eiffel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eiffel.Messaging.Core
{
    public class MiddlewareOptions
    {
        private readonly IServiceCollection _services;

        public MiddlewareOptions()
        {
            _services = new ServiceCollection();
        }

        public MiddlewareOptions Add<TService, TImplementation>()
            where TService : class, IMessagingMiddleware
            where TImplementation : class, TService
        {
            _services.AddSingleton<TService, TImplementation>();
            return this;
        }

        public MiddlewareOptions Add(Type type, object instance)
        {
            _services.AddSingleton(type, instance);
            return this;
        }

        internal List<IMessagingMiddleware> GetMiddlewares()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var middlewares = serviceProvider.GetServices<IMessagingMiddleware>();
            return (middlewares ?? Enumerable.Empty<IMessagingMiddleware>()).ToList();
        }
    }
}
