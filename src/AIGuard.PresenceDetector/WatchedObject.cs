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

        private List<bool> _hits;

        public WatchedObject()
        {

            new WatchedObject(5, false);
        }

        public WatchedObject(int sampleSize, bool initialState)
        {
            _hits = Enumerable.Repeat(initialState, sampleSize).ToList();
        }

        public void AddDiscovery(bool discovery)
        {
            _hits.Insert(0, discovery);
            _hits.RemoveAt(_hits.Count - 1);
        }


    }
}
