using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AIGuard.Broker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AIGuard.PresenceDetector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);


                using (var client = new HttpClient())
                {
                    HttpResponseMessage output = await client.GetAsync("http://192.168.0.4/data/monitor.client.client.json?operation=load&_=1586052514304");
                    var result = JsonConvert.DeserializeObject<IPresence>(await output.Content.ReadAsStringAsync());
                }
            }
        }
    }
}
