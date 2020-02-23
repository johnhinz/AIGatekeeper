using AIGaurd.Broker;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MongoRepository
{
    public class Prediction : IPrediction
    {
        public bool Success { get; set; }
        public string Base64Image { get; set; }
        public string FileName { get; set; }
        public IDetectedObject[] Detections { get; set; }
    }
}
