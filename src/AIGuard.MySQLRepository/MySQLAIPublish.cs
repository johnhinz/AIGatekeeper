using AIGuard.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.MySQLRepository
{
    class MySQLAIPublish : IPublishDetections<bool>
    {
        public Task<bool> PublishAsync<TPredictionType>(TPredictionType message, string source, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
