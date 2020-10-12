using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.IRepository
{
    public interface IPublishDetections<TResult>
    {
        public Task<TResult> PublishAsync<TPredictionType>
            (TPredictionType message, string source, CancellationToken token);
    }
}
