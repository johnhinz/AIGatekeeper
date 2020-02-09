using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIGaurd.Broker
{
    interface IDetectObjects
    {
        public Task DetectObjectsAsync(string imagePath);
    }
}
