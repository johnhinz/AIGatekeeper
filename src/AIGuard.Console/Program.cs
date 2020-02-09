using AIGaurd.DeepStack;
using System;
using System.Threading.Tasks;

namespace AIGuard.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DetectObjects objDetector = new DetectObjects("http://vmhost.johnhinz.com:80/v1/vision/detection");
            var result = await objDetector.DetectObjectsAsync(@"C:\Temp\AIGarage.20200202_164055262.jpg");
        }
    }
}
