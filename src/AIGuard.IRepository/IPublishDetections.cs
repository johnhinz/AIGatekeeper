using AIGuard.Broker;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.IRepository
{
    public interface IPublishDetections<TResult>
    {
        public Task<TResult> PublishAsync
            (IPrediction message, string source, CancellationToken token);
    }
}
