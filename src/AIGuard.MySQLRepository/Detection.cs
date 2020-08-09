using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AIGuard.MySQLRepository
{
    public class Detection
    {
        public int Id { get; set; }
        [Column("label")]
        public string Label { get; set; }
        [Column("confidence")]
        public decimal Confidence { get; set; }
        [Column("xmin")]
        public int XMin { get; set; }
        [Column("xmax")]
        public int XMax { get; set; }
        [Column("ymin")]
        public int YMin { get; set; }
        [Column("ymax")]
        public int YMax { get; set; }
        [Column("capture_id")]
        public int CaptureId { get; set; }
    }
}
