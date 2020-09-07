using AIGuard.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.MySQLRepository
{
    public class PublishDetections : IPublishDetections<int>
    {
        private string _connectionString;

        public PublishDetections(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<int> PublishAsync<TPredictionType>(TPredictionType message, string source, CancellationToken token)
        {
            using (var context = new AIContext(_connectionString))
            {
                context.Entry(message).State = EntityState.Added;
                return context.SaveChangesAsync();
            }
        }
    }


}
