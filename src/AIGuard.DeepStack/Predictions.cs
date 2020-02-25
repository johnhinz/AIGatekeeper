using AIGuard.Broker;
using Newtonsoft.Json;

namespace AIGuard.DeepStack
{
    [JsonObject("predictions")]
    public class Predictions : IPrediction
    {
        public Predictions()
        {
            Detections = new DetectedObject[20];
        }
        [JsonProperty("success")]
        public bool Success { get; set; }
        public string Base64Image { get; set; }
        public string FileName { get; set; }
        [JsonProperty("predictions")]
        public IDetectedObject[] Detections { get; set; }
        
    }
}
