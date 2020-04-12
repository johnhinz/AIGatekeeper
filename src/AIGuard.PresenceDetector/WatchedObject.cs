using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.PresenceDetector
{
    public class WatchedObject
    {
        public string IP { get; set; }
        public string QueSubName { get; set; }
        public string FoundValue { get; set; }
        public string NotFoundValue { get; set; }
    }
}
