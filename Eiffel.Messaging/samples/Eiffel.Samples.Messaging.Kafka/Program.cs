using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Eiffel.Messaging.Core;
using Eiffel.Messaging.Kafka;
using Eiffel.Messaging.Abstractions;
using Eiffel.Samples.Messaging.Kafka.Middlewares;

namespace Eiffel.Samples.Messaging.Kafka
{
    public class Program
    {
       public static async Task Main(string[] args)
        {
            var serviceHost = CreateHostBuilder(args).Build();
            AppDomain.CurrentDomain.UnhandledException += (sender, exception) =>
            {
                var logger = serviceHost.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError((Exception)exception.ExceptionObject, "UnhandledException");
            };
            await serviceHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration(cfg =>
               {
                   var config = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                                        .AddEnvironmentVariables()
                                        .Build();

                   cfg.AddConfiguration(config);
               })
               .UseWindowsService()
               .ConfigureLogging((context, builder) =>
               {
                   builder.AddSimpleConsole(options =>
                   {
                       options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                   });
               })
               .ConfigureServices(ConfigureServices);
            return hostBuilder;
        }

        private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddMediator();
            services.AddEventBus<KafkaClient, KafkaClientConfig>();
            services.AddMessageBus<KafkaClient, KafkaClientConfig>(options =>
            {
                options.AddMiddleware<IMessagingMiddleware, ValidationMiddleware>();
            });
            services.AddHostedService<WorkerService>();
        }
    }
}
