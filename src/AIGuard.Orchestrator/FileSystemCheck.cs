using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;

namespace AIGuard.Orchestrator
{
    internal class FileSystemCheck : IHealthCheck
    {
        private readonly string _path;
        private readonly RetryPolicy _fileAccessRetryPolicy;

        public FileSystemCheck()
        {
            _fileAccessRetryPolicy = Policy
                .Handle<FileLoadException>()
                .Or<FileNotFoundException>()
                .Or<ArgumentException>()
                .Or<OutOfMemoryException>()
                .Retry(3);
        }

        public FileSystemCheck(string path)
        {
            _path = path;
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

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            DirectoryInfo d = new DirectoryInfo(_path);
            FileInfo[] files = d.GetFiles("*.*");
            FileInfo file = files.FirstOrDefault();

            try
            {
                _fileAccessRetryPolicy.Execute(() => { return File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read); });
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}
