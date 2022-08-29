using AIGuard.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=5322557

namespace AIGuard.CodeProject.AI
{
    internal class Predictions : IPrediction
    {
        public bool Success { get ; set ; }
        public string Base64Image { get; set; }
        public string FileName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDetectedObject[] Detections { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
