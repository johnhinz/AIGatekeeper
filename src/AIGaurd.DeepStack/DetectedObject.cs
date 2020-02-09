using AIGaurd.Broker;
using Newtonsoft.Json;
using System;

namespace AIGaurd.DeepStack
{
    public class DetectedObject : IDetectedObject
    {
        [JsonProperty("confidence")]
        public float Confidence { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("y_min")]
        public int YMin { get; set; }
        [JsonProperty("x_min")]
        public int XMin { get; set; }
        [JsonProperty("y_max")]
        public int YMax { get; set; }
        [JsonProperty("x_max")]
        public int XMax { get; set; }
    }
}
