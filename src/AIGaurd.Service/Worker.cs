using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AIGaurd.Broker;
using AIGaurd.IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Publishing;
using Polly;
using Polly.Retry;

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
        private readonly AsyncRetryPolicy _httpRetryPolicy;

        public Worker(ILogger<Worker> logger, IDetectObjects objectDetector, IPublish<MqttClientPublishResult> publisher, IDictionary<string,float> watchedObjects, string imagePath, string watchedExtensions)
        {
            _path = imagePath ?? throw new ArgumentNullException("Worker:imagePath cannot be null.");
            _watchedExtensions = watchedExtensions.Split(';').ToList() ?? throw new ArgumentNullException("Worker:watchedExtensions cannot be null.");

            
            _logger = logger;
            _objDetector = objectDetector;
            _publisher = publisher;
            _watchedObjects = watchedObjects;

            _httpRetryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(100),
                    (ex, timeSpan) =>
                    {
                        _logger.LogError(ex.Message);
                    });
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
                    if (IsFileClosed(e.FullPath, true))
                    {
                        IPrediction result = null;
                        _httpRetryPolicy.ExecuteAndCaptureAsync(async () =>
                            {
                                result = await _objDetector.DetectObjectsAsync(e.FullPath);
                            }).Wait();

                        if (result == null)
                        {
                            _logger.LogError($"Cannot connect to object detector.");
                            return;
                        }
                        if (result.Success && DetectTarget(result.Detections))
                        {
                            result.base64Image = Convert.ToBase64String(File.ReadAllBytes(e.FullPath));
                            _httpRetryPolicy.ExecuteAsync(async () =>
                            {
                                await _publisher.PublishAsync(result, e.Name, CancellationToken.None);
                            });
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

        private bool DetectTarget(IDetectedObject[] items)
        {
            if (!items.Any(d => _watchedObjects.ContainsKey(d.Label)))
                return false;
            bool targetFound = false;
            foreach (var detection in items)
                if (_watchedObjects.ContainsKey(detection.Label))
                    if (targetFound = detection.Confidence >= _watchedObjects[detection.Label])
                        break;
            return targetFound;
        }

        private bool IsFileClosed(string filepath, bool wait)
        {
            bool fileClosed = false;
            int retries = 20;
            const int delayMS = 100;

            if (!File.Exists(filepath))
                return false;
            do
            {
                try
                {
                    FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    fs.Close();
                    fileClosed = true; // success
                }
                catch (IOException) { }

                if (!wait) break;

                retries--;

                if (!fileClosed)
                    Thread.Sleep(delayMS);
            }
            while (!fileClosed && retries > 0);
            return fileClosed;
        }
    }
}
