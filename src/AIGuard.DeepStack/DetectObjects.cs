using AIGuard.Broker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIGuard.DeepStack
{
    public class DetectObjects : IDetectObjects
    {
        private readonly ILogger<DetectObjects> _logger;
        private readonly string _endPoint;
        public DetectObjects(ILogger<DetectObjects> logger, string endPoint)
        {
            _logger = logger;
            _endPoint = endPoint;
        }
        public async Task<IPrediction> DetectObjectsAsync(byte[] image, string imagePath)
        {
            _logger.LogInformation($"DetectObjectsAsync called for {imagePath}");
            using (MemoryStream ms = new MemoryStream(image))
            {
                HttpResponseMessage output;
                using (var _client = new HttpClient())
                {
                    var request = new MultipartFormDataContent();
                    request.Add(new StreamContent(ms), "image", Path.GetFileName(imagePath));
                    output = await _client.PostAsync(_endPoint, request);
                }
                return JsonConvert.DeserializeObject<Predictions>(await output.Content.ReadAsStringAsync());
            }
           
        }
    }
}
