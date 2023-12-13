using AIGuard.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGuard.CodePorject.AI
{
    //https://www.codeproject.com/AI/docs/api/api_reference.html

    internal class DetectObjects : IDetectObjects
    {
        public Task<IPrediction> DetectObjectsAsync(byte[] image, string imagePath)
        {
            throw new NotImplementedException();
        }
    }
}
