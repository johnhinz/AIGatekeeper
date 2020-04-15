using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIGuard.PresenceDetector
{
    public class WatchedObject
    {
        public string IP { get; set; }
        public string QueSubName { get; set; }
        public string FoundValue { get; set; }
        public string NotFoundValue { get; set; }
        protected int Tolerance { get; set; }
        private FixedSizedQueue _hits = new FixedSizedQueue(20);

        public bool Available
        {
            get 
            {
                    return _hits.Available; 
            }
        }

        public void AddDiscovery(bool discovery)
        {
            _hits.Enqueue(discovery);
        }

        private class FixedSizedQueue
        {
            private readonly int _tolerance;
            private ConcurrentQueue<bool> _queue = new ConcurrentQueue<bool>();
            private object lockObject = new object();

            public bool Available
            {
                get
                {
                    if (_queue != null)
                    {

                        return _queue.Any(h => h == true);
                    }
                    else
                    {
                        return false;
                    }
                }
            }


            public FixedSizedQueue(int size)
            {
                _tolerance = size;
            }
          
            public void Enqueue(bool obj)
            {
                _queue.Enqueue(obj);
                lock (lockObject)
                {
                    bool overflow;
                    while (_queue.Count > _tolerance && _queue.TryDequeue(out overflow)) ;
                }
            }
        }
    }
}
