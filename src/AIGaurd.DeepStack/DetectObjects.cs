using AIGaurd.Broker;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIGaurd.DeepStack
{
    public class DetectObjects : IDetectObjects
    {
        private readonly string _endPoint;
        public string ImagePath { get; set; }
        public DetectObjects(string endPoint)
        {
           _endPoint = endPoint;
        }
        public async Task<IPrediction> DetectObjectsAsync(string imagePath)
        {
            string jsonString = string.Empty;
            var request = new MultipartFormDataContent();
            ImagePath = imagePath;
            HttpResponseMessage output;
            using (var _client = new HttpClient())
            {
                using (var image_data = File.OpenRead(imagePath))
                {
                    request.Add(new StreamContent(image_data), "image", Path.GetFileName(imagePath));
                    output = await _client.PostAsync(_endPoint, request);
                }
            }
            return JsonConvert.DeserializeObject<Predictions>(await output.Content.ReadAsStringAsync());
        }
    }
}
