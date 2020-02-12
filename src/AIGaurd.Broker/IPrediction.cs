using System.Collections.Generic;

namespace AIGaurd.Broker
{
    public interface IPrediction
    {
        public bool Success { get; set; }
        public string base64Image { get; set; }
        public IDetectedObject[] Detections { get; set; }

    }
}
