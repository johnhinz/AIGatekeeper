using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIGaurd.Broker
{
    public interface IDetectObjects 
    {
        public Task<IPredictions> DetectObjectsAsync(string imagePath);
    }
}
