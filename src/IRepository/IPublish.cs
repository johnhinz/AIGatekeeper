using AIGaurd.Broker;
using System.Threading;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IPublish
    {
        public Task<TResult> PublishAsync<TResult>
            (IPrediction message, string source, CancellationToken token);
    }
}
