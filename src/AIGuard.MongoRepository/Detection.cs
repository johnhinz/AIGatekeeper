using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MongoRepository
{
    public class Detection
    {
        public double Confidence { get; set; }
        public string Label { get; set; }
        public int YMin { get; set; }
        public int XMin { get; set; }
        public int YMax { get; set; }
        public int XMax { get; set; }
    }
}
