using AIGaurd.Broker;
using System.Threading;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IPublish<TResult>
    {
        public Task<TResult> PublishAsync
            (IPrediction message, string source, CancellationToken token);
    }
}
