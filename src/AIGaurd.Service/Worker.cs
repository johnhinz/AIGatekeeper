using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIGaurd.Broker;
using AIGaurd.DeepStack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;

namespace AIGaurd.Service
{
    public class Worker : BackgroundService
    {
        private readonly string _path;
        private readonly ILogger<Worker> _logger;
        private readonly IDetectObjects _objDetector;

        public Worker(ILogger<Worker> logger, IDetectObjects objectDetector, string imagePath)
        {
            _path = imagePath;
            _logger = logger;
            _objDetector = objectDetector;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (FileSystemWatcher dirWatcher = new FileSystemWatcher())
            {
                dirWatcher.Path = _path;
                dirWatcher.NotifyFilter = NotifyFilters.FileName;
                dirWatcher.Filter = "*.jpg";
                dirWatcher.Created += OnChanged;
                dirWatcher.EnableRaisingEvents = true;

                while (!stoppingToken.IsCancellationRequested)
                {
                    
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"OnChange event start: {e.FullPath} {DateTime.Now}");
            
            
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithClientId("Client1")
                .WithTcpServer("vmhost.johnhinz.com")
                .WithCleanSession()
                .Build();

            mqttClient.ConnectAsync(options, CancellationToken.None).Wait();

            var result = _objDetector.DetectObjectsAsync(e.FullPath).Result;
            
            foreach (var item in result.Detections)
            {
                _logger.LogInformation($"{item.Label} {item.Confidence}");
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"AI/{e.Name.Split('.')[0]}/{e.Name}")
                    .WithPayload(JsonSerializer.Serialize<IPrediction>(result))
                    .Build();
                mqttClient.PublishAsync(message, CancellationToken.None);
            }
            if (result.Detections.Count() == 0)
            {
                _logger.LogInformation($"No objects found in {e.FullPath}");
            }
            _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}");
        }
    }
}
