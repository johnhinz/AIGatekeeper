using System;
using System.Collections.Generic;
using System.Text;

namespace AIGaurd.Broker
{
    public interface IDetectedObject
    {
        public float Confidence { get; set; }
        public string Label { get; set; }
        public int YMin { get; set; }
        public int XMin { get; set; }
        public int YMax { get; set; }
        public int XMax { get; set; }
    }
}
