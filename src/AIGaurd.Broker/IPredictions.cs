using System;
using System.Collections.Generic;
using System.Text;

namespace AIGaurd.Broker
{
    public interface IPredictions
    {
        public bool Success { get; set; }
        public IDetectedObject[] Detections { get; set; }

    }
}
