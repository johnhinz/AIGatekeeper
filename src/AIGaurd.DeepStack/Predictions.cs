using AIGaurd.Broker;
using Newtonsoft.Json;

namespace AIGaurd.DeepStack
{
    [JsonObject("predictions")]
    public class Predictions : IPredictions
    {
        public Predictions()
        {
            Detections = new DetectedObject[20];
        }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("predictions")]
        public IDetectedObject[] Detections { get; set; }
    }
}
