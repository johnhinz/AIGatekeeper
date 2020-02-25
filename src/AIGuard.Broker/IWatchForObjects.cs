using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Broker
{
    public interface IWatchForObjects
    {
        public string Topic { get; set; }
        public ICollection<IWatchedObject> WatchedObjects { get; set; }
    }
}
