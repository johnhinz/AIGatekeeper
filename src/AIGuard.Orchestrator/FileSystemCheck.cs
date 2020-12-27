using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AIGuard.Orchestrator
{
    internal class FileSystemCheck : IHealthCheck
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

        public static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString())))))));

            return httpContext.Response.WriteAsync(response.ToString(Formatting.Indented));
        }
    }
}
