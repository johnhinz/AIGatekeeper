using AIGaurd.Broker;
using AIGaurd.DeepStack;
using AIGaurd.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Publishing;
using AIGuard.MqttRepository;
using System.Collections.Generic;

namespace AIGaurd.Service
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
                    services.AddTransient<IDetectObjects, DetectObjects>((serviceProvider) =>
                         {
                             return new DetectObjects(hostContext.Configuration.GetSection("AIEndpoint").Value); 
                         }
                    ) ;
                    services.AddTransient<IPublish<MqttClientPublishResult>>((serviceProvider) =>
                    {
                        return new MqttPublish(
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
                                serviceProvider.GetService<IDetectObjects>(),
                                serviceProvider.GetService<IPublish<MqttClientPublishResult>>(),
                                hostContext.Configuration.GetSection("WatchedObjects").Get<Dictionary<string,float>>(),
                                hostContext.Configuration.GetSection("WatchFolder").Value,
                                hostContext.Configuration.GetSection("WatchedExtensions").Value);
                        });
                });
    }
}
