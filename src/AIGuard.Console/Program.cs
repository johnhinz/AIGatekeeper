using AIGaurd.DeepStack;
using System.Threading.Tasks;

namespace AIGuard.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DetectObjects objDetector = new DetectObjects("http://vmhost.johnhinz.com:80/v1/vision/detection", @"C:\Temp");
            var result = await objDetector.DetectObjectsAsync(@"C:\Temp\AIWest.20200208_220143497.jpg");
            foreach (var item in result.Detections)
            {
                System.Console.WriteLine($"{item.Label} {item.Confidence}");
            }
            System.Console.ReadLine();
        }
    }
}
