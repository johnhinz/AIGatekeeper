using AIGuard.Broker;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.IRepository
{
    interface IPublishPresence<T>
    {
        public Task<T> PublishAsync
            (IPresence message, string source, CancellationToken token);
    }
}
