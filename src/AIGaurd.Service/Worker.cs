using System;
using System.Collections.Generic;
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
        private readonly IPublish<MqttClientPublishResult> _publisher;
        private readonly List<string> _watchedExtensions;

        public Worker(ILogger<Worker> logger, IDetectObjects objectDetector, IPublish<MqttClientPublishResult> publisher, string imagePath, string watchedExtensions)
        {
            _path = imagePath;
            _logger = logger;
            _objDetector = objectDetector;
            _publisher = publisher;
            _watchedExtensions = watchedExtensions.Split(';').ToList();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (FileSystemWatcher dirWatcher = new FileSystemWatcher())
            {
                dirWatcher.Path = _path;
                dirWatcher.NotifyFilter = NotifyFilters.FileName;
                dirWatcher.Filter = "*.*";
                dirWatcher.Created += OnChanged;
                dirWatcher.EnableRaisingEvents = true;

                while (!stoppingToken.IsCancellationRequested) { }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"OnChange event start: {e.FullPath} {DateTime.Now}");

            if (_watchedExtensions.Any(ext => e.Name.EndsWith(ext)))
            {
                var result = _objDetector.DetectObjectsAsync(e.FullPath).Result;
                if (result.Success)
                {
                    result.base64Image = Convert.ToBase64String(File.ReadAllBytes(e.FullPath));
                    _publisher.PublishAsync(result, e.Name, CancellationToken.None);
                }
            }
            _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}");
        }
    }
}
