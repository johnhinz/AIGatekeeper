using System.Threading.Tasks;

namespace AIGuard.Broker
{
    public interface IDetectObjects 
    {
        public Task<IPrediction> DetectObjectsAsync(string imagePath);
    }
}
