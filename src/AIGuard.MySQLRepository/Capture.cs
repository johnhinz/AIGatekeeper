using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AIGuard.MySQLRepository
{
    public class Capture
    {
        public int Id { get; set; }
        public DateTime dt { get; set; }
        [Column("success")]
        public bool Success { get; set; }
        [Column("filename")]
        public string FileName { get; set; }
        [Column("image")]
        public string Base64Image { get; set; }
        public virtual List<Detection> Detections { get; set; }
    }
}
