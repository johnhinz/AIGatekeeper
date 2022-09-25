using AIGuard.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGuard.CodePorject.AI
{
    internal class DetectedObject : IDetectedObject
    {
        public float Confidence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int YMin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int XMin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int YMax { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int XMax { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
