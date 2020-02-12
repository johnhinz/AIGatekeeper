using AIGaurd.Broker;
using Newtonsoft.Json;

namespace AIGaurd.DeepStack
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
        public string base64Image { get; set; }
        [JsonProperty("predictions")]
        public IDetectedObject[] Detections { get; set; }
        
    }
}
