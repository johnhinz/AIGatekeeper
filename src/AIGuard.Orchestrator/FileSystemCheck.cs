using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Permissions;

namespace AIGuard.Orchestrator
{
    class FileSystemCheck : IHealthCheck
    {
        private readonly string _path;

        public FileSystemCheck(string path)
        {
            _path = path;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            DirectoryInfo d = new DirectoryInfo(_path);
            FileInfo[] files = d.GetFiles("*.*");
            FileInfo file =  files.FirstOrDefault();

            if (FileHelper.IsFileClosed(file.FullName, false))
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}
