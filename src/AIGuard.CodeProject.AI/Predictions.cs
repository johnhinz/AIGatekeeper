using AIGuard.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGuard.CodePorject.AI
{
    internal class Predictions : IPrediction
    {
        public bool Success { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Base64Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string FileName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDetectedObject[] Detections { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
