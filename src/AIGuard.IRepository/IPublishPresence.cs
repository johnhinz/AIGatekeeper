using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.IRepository
{
    interface IPublishPresence<T>
    {
        public Task<T> PublishAsync
            (IPrediction message, string source, CancellationToken token);
    }
}
