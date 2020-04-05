using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Broker
{
    public interface IPresence
    {
        public int Error { get; set; }
        public bool Success { get; set; }
        public string Timeout { get; set; }
        public List<IExist> Exists { get; set; }
    }
}

