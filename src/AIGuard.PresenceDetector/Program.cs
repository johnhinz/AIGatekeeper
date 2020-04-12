using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIGuard.IRepository;
using AIGuard.MqttRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Publishing;

namespace AIGuard.PresenceDetector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IPublishDetections<MqttClientPublishResult>>((serviceProvider) =>
                    {
                        return new MqttAIPublish(
                            hostContext.Configuration.GetSection("RepositoryEndpoint").Value,
                            hostContext.Configuration.GetSection("PublisherName").Value,
                            hostContext.Configuration.GetSection("TopicParser").Value,
                            int.Parse(hostContext.Configuration.GetSection("TopicPosition").Value),
                            hostContext.Configuration.GetSection("QueueName").Value);
                    });

                    services.AddHostedService<Worker>((serviceProvider) =>
                    {
                        return new Worker(
                                serviceProvider.GetService<ILogger<Worker>>(),
                                hostContext.Configuration.GetSection("IPRange").Get<Dictionary<string, string>>(),
                                serviceProvider.GetService<IPublishDetections<MqttClientPublishResult>>(),
                                int.Parse(hostContext.Configuration.GetSection("CheckFrequency").Value),
                                hostContext.Configuration.GetSection("WatchedObjects").Get<Dictionary<string, WatchedObject>>()
                            );
                    });
                });
    }
}
