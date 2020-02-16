using System;
using System.Collections.Generic;
using System.Text;

namespace AIGaurd.Broker
{
    public interface IWatchedObject
    {
        public float Confidence { get; set; }
        public string Label { get; set; }
    }
}
