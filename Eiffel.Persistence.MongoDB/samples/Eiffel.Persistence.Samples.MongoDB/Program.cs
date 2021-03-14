using Eiffel.Persistence.MongoDB;
using Eiffel.Persistence.Samples.MongoDB.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eiffel.Persistence.Samples.MongoDB
{
    public class Program
    {
        protected Program() { }

        public static async Task Main(string[] args)
        {
            var serviceHost = CreateHostBuilder(args).Build();
            AppDomain.CurrentDomain.UnhandledException += (sender, exception) =>
            {
                var logger = serviceHost.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError((Exception)exception.ExceptionObject, "UnhandledException");
            };

            var dbContext = serviceHost.Services.GetRequiredService<SampleDbContext>();
            var users = dbContext.Users.ToList();
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
               .ConfigureServices(ConfigureServices);
            return hostBuilder;
        }

        private static void ConfigureServices(HostBuilderContext builderContext, IServiceCollection services)
        {
            services.AddDbContext(new DbContextOptions<SampleDbContext>("127.0.0.1", 27017, "eiffel"));
        }
    }
}
