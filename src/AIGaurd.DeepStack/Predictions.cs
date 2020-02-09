using AIGaurd.Broker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGaurd.DeepStack
{
    [JsonObject("predictions")]
    public class Predictions : IPredictions
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("predictions")]
        public IDetectedObject[] Detections { get; set; }
    }
}
