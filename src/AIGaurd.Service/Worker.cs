using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIGaurd.Broker;
using AIGaurd.DeepStack;
using IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;

namespace AIGaurd.Service
{
    public class Worker : BackgroundService
    {
        private readonly string _path;
        private readonly ILogger<Worker> _logger;
        private readonly IDetectObjects _objDetector;
        private readonly IPublish _publisher;

        public Worker(ILogger<Worker> logger, IDetectObjects objectDetector, IPublish publisher, string imagePath)
        {
            _path = imagePath;
            _logger = logger;
            _objDetector = objectDetector;
            _publisher = publisher;
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

            var result = _objDetector.DetectObjectsAsync(e.FullPath).Result;
            if (result.Success)
            {
                byte[] imageArray = File.ReadAllBytes(e.FullPath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                result.base64Image = base64ImageRepresentation;


                _publisher.PublishAsync<MqttClientPublishResult>(result, e.Name, CancellationToken.None);

                
            }
            
            _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}");
        }
    }
}
