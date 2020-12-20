using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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

namespace AIGuard.Orchestrator
{
    public class Worker : BackgroundService
    {
        private readonly string _path;
        private readonly ILogger<Worker> _logger;
        private readonly IDetectObjects _objDetector;
        private readonly IPublishDetections<MqttClientPublishResult> _publisher;
        private readonly List<string> _watchedExtensions;
        private readonly bool _publishFalseDetections;
        private readonly Stopwatch _stopwatch;
        private readonly IEnumerable<Camera> _cameras;
        private readonly AsyncRetryPolicy _httpRetryPolicy;

        private const string falseDetectionTopic = "False";

        public Worker(
            ILogger<Worker> logger, 
            IDetectObjects objectDetector, 
            IPublishDetections<MqttClientPublishResult> publisher, 
            IEnumerable<Camera> cameras, 
            string imagePath, 
            string watchedExtensions,
            bool publishFalseDetections)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _objDetector = objectDetector ?? throw new ArgumentNullException(nameof(objectDetector)); 
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _cameras = cameras ?? throw new ArgumentNullException(nameof(cameras));
            _path = imagePath ?? throw new ArgumentNullException(nameof(imagePath));
            _watchedExtensions = watchedExtensions?.Split(';').ToList() ?? throw new ArgumentNullException(nameof(watchedExtensions));
            _publishFalseDetections = publishFalseDetections;

            _stopwatch = new Stopwatch();

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

                _logger.LogInformation("========== Orchestrator Start Up >>>>>>>>>>>>");

                while (!stoppingToken.IsCancellationRequested) 
                {
                    Thread.Sleep(100);
                }
                dirWatcher.Created -= OnChanged;
                _logger.LogInformation("<<<<<<<<<< Orchestrator Shut Down ==========");

            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _stopwatch.Start();
            try
            {
                _logger.LogInformation($"OnChange event start: {e.FullPath} {DateTime.Now}");
                try
                {

                    Camera camera = null;
                    try
                    {
                        camera = FindCamera(e);
                    }
                    catch (ArgumentOutOfRangeException exp)
                    {
                        _logger.LogError(exp.Message);
                        return;
                    }

                    if (IsFileClosed(e.FullPath, true))
                    {
                        using (Image image = Image.FromFile(e.FullPath))
                        {
                            _logger.LogInformation($"Checking file {e.FullPath}.");

                            IPrediction result = DetectObjectAsync(image, e.FullPath).Result;
                            if (result == null)
                            {
                                _logger.LogError($"Cannot connect to object detector.");
                                return;
                            }


                            bool foundTarget = DetectTarget(camera, result.Detections);

                            if ((result.Success && foundTarget) || (result.Detections.Count() > 0 && _publishFalseDetections))
                            {
                                _logger.LogInformation($"{result.Detections.Count()} target(s) found in {e.FullPath}.");
                                result.FileName = e.Name;

                                string topic = foundTarget ? e.Name : falseDetectionTopic;
                                if (camera.Clip)
                                {
                                    List<MemoryStream> streams = CropBounds(image, result, camera);
                                    foreach (MemoryStream ms in streams)
                                    {
                                        using (ms)
                                        {
                                            result.Base64Image = Convert.ToBase64String(ms.ToArray());
                                            PublishAsync(result, topic, CancellationToken.None).Wait();
                                        }
                                    }
                                }
                                else
                                {
                                    using (MemoryStream ms = DrawBounds(image, result, camera))
                                    {
                                        result.Base64Image = Convert.ToBase64String(ms.ToArray());
                                        PublishAsync(result, topic, CancellationToken.None).Wait();
                                    }
                                }
                            }

                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError($"Unable to connect to IDetectObjects:{typeof(IDetectObjects)}:{ex.Message}");
                }
            }
            finally
            {
                _stopwatch.Stop();
                _logger.LogInformation($"OnChange event end: {e.FullPath} {DateTime.Now}, elapsed time:{_stopwatch.Elapsed.TotalSeconds}");
                _logger.LogInformation("_____________________________________");
                _stopwatch.Reset();
            }
        }

        private List<MemoryStream> CropBounds(Image image, IPrediction result, Camera camera)
        {
            List<MemoryStream> streams = new List<MemoryStream>();
            foreach (IDetectedObject detection in result.Detections)
            {
                _logger.LogInformation($"Found item: {detection.Label}, confidence: {detection.Confidence} at x:{detection.XMin} y:{detection.YMin} xmax:{detection.XMax} ymax:{detection.YMax}");

                if (camera.Watches.Any(i => i.Label == detection.Label))
                {
                    Rectangle cropRect = new Rectangle(detection.XMin, detection.YMin, detection.XMax - detection.XMin, detection.YMax - detection.YMin);
                    Bitmap src = image as Bitmap;
                    Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);


                    using (Graphics g = Graphics.FromImage(target))
                    {

                        g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                         cropRect,
                                         GraphicsUnit.Pixel);

                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        target.Save(ms, image.RawFormat);
                        streams.Add(ms);
                    }
                }
            }
            return streams;
        }

        private MemoryStream DrawBounds(Image image, IPrediction result, Camera camera)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                using (Pen redPen = new Pen(Color.Red, 5))
                using (Font font = new Font("Arial", 30, FontStyle.Italic, GraphicsUnit.Pixel))
                using (SolidBrush brush = new SolidBrush(Color.White)) 

                    foreach (IDetectedObject detection in result.Detections)
                    {
                        _logger.LogInformation($"Found item: {detection.Label}, confidence: {detection.Confidence} at x:{detection.XMin} y:{detection.YMin} xmax:{detection.XMax} ymax:{detection.YMax}");
                        if (camera.Watches.Any(i => i.Label == detection.Label))
                        {
                            g.DrawRectangle(
                                redPen,
                                detection.XMin,
                                detection.YMin,
                                detection.XMax - detection.XMin,
                                detection.YMax - detection.YMin);

                            g.DrawString($"{detection.Label}:{detection.Confidence}",
                                font,
                                brush,
                                new Point(detection.XMin, detection.YMin - (int)redPen.Width - 1));
                        }
                    }
                MemoryStream ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                return ms;
            
            }
        }

        public Camera FindCamera(FileSystemEventArgs e)
        {
            foreach (var item in _cameras)
            {
                if (e.Name.Contains(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug($"Found camera {item.Name} for file {e.Name}");
                    return item;
                }
            }
            throw new ArgumentOutOfRangeException($"Camera for {e.FullPath} not found");
        }

        private async Task<MqttClientPublishResult> PublishAsync(IPrediction prediction, string fileName, CancellationToken token)
        {
            return await _httpRetryPolicy.ExecuteAsync(() => _publisher.PublishAsync(prediction, fileName, token))
                .ConfigureAwait(false);
        }

        private async Task<IPrediction> DetectObjectAsync(Image image, string filePath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return await _httpRetryPolicy.ExecuteAsync(() => _objDetector.DetectObjectsAsync(ms.ToArray(), filePath))
                    .ConfigureAwait(false);
            }
        }

        public static bool DetectTarget(Camera camera, IDetectedObject[] detectedItems)
        {
            if (camera == null || detectedItems == null)
            {
                string msg = $"Parameter {(camera == null ? "camera" : string.Empty)} {(detectedItems == null ? "detectedItems" : string.Empty)} cannot be null";
                throw new ArgumentNullException(paramName: msg);
            }

            if (!detectedItems.Any(i => camera.Watches.Any(w => w.Label == i.Label)))
                return false;

            foreach (var detection in detectedItems)
            {
                Item item = camera.Watches.FirstOrDefault(w => w.Label == detection.Label && w.Confidence <= detection.Confidence);
                if (item != null)
                {
                    return true;
                }

            }
            return false;
        }

   

        private static bool IsFileClosed(string filepath, bool wait)
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
