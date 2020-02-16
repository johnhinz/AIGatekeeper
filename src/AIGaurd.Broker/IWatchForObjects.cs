using System;
using System.Collections.Generic;
using System.Text;

namespace AIGaurd.Broker
{
    public interface IWatchForObjects
    {
        public string Topic { get; set; }
        public ICollection<IWatchedObject> WatchedObjects { get; set; }
    }
}
