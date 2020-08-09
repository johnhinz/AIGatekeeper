using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MySQLRepository
{
    class Detection
    {
        public int Id { get; set; }
        public string Target { get; set; }
        public decimal Confidence { get; set; }
        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
    }
}
