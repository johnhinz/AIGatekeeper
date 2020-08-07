using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MySQLRepository
{
    class Capture
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string FileName { get; set; }
        public virtual List<Detection> Detections { get; set; }
    }
}
