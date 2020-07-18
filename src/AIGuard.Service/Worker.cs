using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AIGuard.Broker;
using AIGuard.IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Publishing;
using Polly;
using Polly.Retry;

namespace AIGuard.Service
{
    public class Worker : BackgroundService
    {
        private readonly string _path;
        private readonly ILogger<Worker> _logger;
        private readonly IDetectObjects _objDetector;
        private readonly IPublishDetections<MqttClientPublishResult> _publisher;
        private readonly List<string> _watchedExtensions;
        private readonly IDictionary<string, float> _watchedObjects;
        private readonly AsyncRetryPolicy _httpRetryPolicy;

        private const string falseDetectionTopic = "False";

        public Worker(
            ILogger<Worker> logger, 
            IDetectObjects objectDetector, 
            IPublishDetections<MqttClientPublishResult> publisher, 
            IDictionary<string,float> watchedObjects, 
            string imagePath, 
            string watchedExtensions)
        {
            _logger = logger;
            _objDetector = objectDetector;
            _publisher = publisher;
            _watchedObjects = watchedObjects;
            _path = imagePath ?? throw new ArgumentNullException("Worker:imagePath cannot be null.");
            _watchedExtensions = watchedExtensions.Split(';').ToList() ?? throw new ArgumentNullException("Worker:watchedExtensions cannot be null.");
            
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
                foreach (var ext in _watchedExtensions)
                {
                    dirWatcher.Filters.Add(ext);
                }
                dirWatcher.Created += OnChanged;
                dirWatcher.EnableRaisingEvents = true;

                while (!stoppingToken.IsCancellationRequested) 
                {
                    Thread.Sleep(100);
                }

                dirWatcher.Created -= OnChanged;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"OnChange event start: {e.FullPath} {DateTime.Now}");

            try
            {
                if (IsFileClosed(e.FullPath, true))
                {
                    _logger.LogInformation($"Checking file {e.FullPath}.");
                    IPrediction result = DetectObjectAsync(e.FullPath).Result;
                    if (result == null)
                    {
                        _logger.LogError($"Cannot connect to object detector.");
                        return;
                    }
                    bool foundTarget = DetectTarget(result.Detections);
                    if ((result.Success && foundTarget) ||
                        (result.Detections.Count() > 0))
                    {
                        result.FileName = e.Name;
                        _logger.LogInformation($"{result.Detections.Count()} target(s) found in {e.FullPath}.");

                        Image image = Image.FromFile(e.FullPath);
                        using (Graphics g = Graphics.FromImage(image))
                        {

                            Pen redPen = new Pen(Color.Red, 5);
                            foreach (IDetectedObject detectedObject in result.Detections)
                            {
                                if (_watchedObjects.ContainsKey(detectedObject.Label))
                                {
                                    g.DrawRectangle(
                                        redPen,
                                        detectedObject.XMin,
                                        detectedObject.YMin,
                                        detectedObject.XMax - detectedObject.XMin,
                                        detectedObject.YMax - detectedObject.YMin);
                                }
                            }

                            using (var ms = new MemoryStream())
                            {
                                image.Save(ms, image.RawFormat);
                                result.Base64Image = Convert.ToBase64String(ms.ToArray());
                            }
                        }
                        string topic = foundTarget ? e.Name : falseDetectionTopic;
                        PublishAsync(result, topic, CancellationToken.None).Wait();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Unable to connect to IDetectObjects:{typeof(IDetectObjects)}:{ex.Message}");
            }
            
            _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}");
        }

        private async Task<MqttClientPublishResult> PublishAsync(IPrediction prediction, string fileName, CancellationToken token)
        {
            _logger.LogInformation($"Publishing {fileName}");
            return await _httpRetryPolicy.ExecuteAsync<MqttClientPublishResult>(() => _publisher.PublishAsync(prediction, fileName, token));
        }

        private async Task<IPrediction> DetectObjectAsync(string filePath)
        {
            return await _httpRetryPolicy.ExecuteAsync<IPrediction>(
                () => _objDetector.DetectObjectsAsync(filePath));
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
