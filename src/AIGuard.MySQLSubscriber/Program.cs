using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIGuard.MySQLSubscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config.AddUserSecrets<Program>())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>((serviceProvider) =>
                    {
                        return new Worker(
                            serviceProvider.GetService<ILogger<Worker>>(),
                            hostContext.Configuration.GetSection("ConnectionStrings:MySQL").Value,
                            hostContext.Configuration.GetSection("ConnectionStrings:MQTT").Value);
                    });
                    

                });
    }
}
