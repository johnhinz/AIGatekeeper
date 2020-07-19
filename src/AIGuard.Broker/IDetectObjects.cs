using System.Threading.Tasks;

namespace AIGuard.Broker
{
    public interface IDetectObjects 
    {
        public Task<IPrediction> DetectObjectsAsync(byte[] image, string imagePath);
    }
}
