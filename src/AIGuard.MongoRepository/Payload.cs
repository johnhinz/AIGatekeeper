using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MongoRepository
{
    public class Payload
    {
        public bool Success { get; set; }
        public string Base64Image { get; set; }
        public List<Detection> Detections { get; set; }
    }
}
