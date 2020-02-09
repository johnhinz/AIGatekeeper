using AIGaurd.Broker;
using AIGaurd.DeepStack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IDetectObjects, DetectObjects>((serviceProvider) =>
                         {
                             return new DetectObjects(hostContext.Configuration.GetSection("AIEndpoint").Value); 
                         }
                    ) ;
                    services.AddHostedService<Worker>((serviceProvider) =>
                        {
                            return new Worker(
                                serviceProvider.GetService<ILogger<Worker>>(), 
                                serviceProvider.GetService<IDetectObjects>(),
                                hostContext.Configuration.GetSection("WatchFolder").Value);
                        }
                        );
                });
    }
}
