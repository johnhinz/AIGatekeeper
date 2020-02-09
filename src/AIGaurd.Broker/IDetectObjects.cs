using System.Threading.Tasks;

namespace AIGaurd.Broker
{
    public interface IDetectObjects 
    {
        public Task<IPrediction> DetectObjectsAsync(string imagePath);
    }
}
