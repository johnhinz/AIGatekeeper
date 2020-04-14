using System;
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
        public int Tolerance { get; set; }

        public bool Avalaible
        {
            get 
            {
                if (_hits != null)
                {

                    return _hits.Any(h => h == true);
                }
                else
                {
                    return false;
                }
            }
        }

        private readonly int _tolorance;
        private List<bool> _hits = Enumerable.Repeat(false, 10).ToList();

        public WatchedObject()
        {
            new WatchedObject(10, false);
        }

        public WatchedObject(int tolorance, bool initialState)
        {
            _tolorance = tolorance;
            _hits = Enumerable.Repeat(initialState, tolorance).ToList();
        }

        public void AddDiscovery(bool discovery)
        {
            if (_hits.Count > _tolorance)
            {
                _hits.RemoveAt(_hits.Count - 1);
            }
            _hits.Insert(0, discovery);
        }
    }
}
