using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIGaurd.Broker;
using AIGaurd.DeepStack;
using AIGaurd.IRepository;
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
        private readonly IDictionary<string, float> _watchedObjects;

        public Worker(ILogger<Worker> logger, IDetectObjects objectDetector, IPublish<MqttClientPublishResult> publisher, IDictionary<string,float> watchedObjects, string imagePath, string watchedExtensions)
        {
            _path = imagePath;
            _logger = logger;
            _objDetector = objectDetector;
            _publisher = publisher;
            _watchedExtensions = watchedExtensions.Split(';').ToList();
            _watchedObjects = watchedObjects;
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

                while (!stoppingToken.IsCancellationRequested) 
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"OnChange event start: {e.FullPath} {DateTime.Now}");

            if (_watchedExtensions.Any(ext => e.Name.EndsWith(ext)))
            {
                try
                {
                    IsFileClosed(e.FullPath, true);

                    IPrediction result = _objDetector.DetectObjectsAsync(e.FullPath).Result;
                    if (result.Success)
                    {
                        // nothing found worth notifying
                        if (!result.Detections.Any(d => _watchedObjects.ContainsKey(d.Label)))
                        {
                            return;
                        }

                        // confirm confidence is above lower limit
                        bool targetFound = false;
                        foreach (var detection in result.Detections)
                        {
                            if(_watchedObjects.ContainsKey(detection.Label))
                            {
                                if (targetFound = detection.Confidence >= _watchedObjects[detection.Label])
                                {
                                    targetFound = true;
                                    break;
                                }
                            }
                        }
                        if (targetFound)
                        {
                            result.base64Image = Convert.ToBase64String(File.ReadAllBytes(e.FullPath));
                            _publisher.PublishAsync(result, e.Name, CancellationToken.None);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError($"Unable to connect to IDetectObjects:{typeof(IDetectObjects)}:{ex.Message}");
                }

            }
            _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}");
        }

        private bool IsFileClosed(string filepath, bool wait)
        {
            bool fileClosed = false;
            int retries = 20;
            const int delay = 500; // Max time spent here = retries*delay milliseconds

            if (!File.Exists(filepath))
                return false;
            do
            {
                try
                {
                    // Attempts to open then close the file in RW mode, denying other users to place any locks.
                    FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    fs.Close();
                    fileClosed = true; // success
                }
                catch (IOException) { }

                if (!wait) break;

                retries--;

                if (!fileClosed)
                    Thread.Sleep(delay);
            }
            while (!fileClosed && retries > 0);
            return fileClosed;
        }
    }
}
