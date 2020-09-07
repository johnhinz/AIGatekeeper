using AIGuard.IRepository;
using AIGuard.MySQLRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Serilog;

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
                .ConfigureLogging((hostContext,logging) =>
                {
                    var serilogLogger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(hostContext.Configuration.GetSection("LogFile").Value)
                        .CreateLogger();

                    logging.ClearProviders();
                    logging.AddSerilog(serilogLogger);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>((serviceProvider) =>
                    {
                        return new Worker(
                            serviceProvider.GetService<ILogger<Worker>>(),
                            new PublishDetections(hostContext.Configuration.GetSection("ConnectionStrings:MySQL").Value),
                            new MqttFactory(),
                            hostContext.Configuration.GetSection("ConnectionStrings:MQTT").Value);
                    });
                });
    }
}
