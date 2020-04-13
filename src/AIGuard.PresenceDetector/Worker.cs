using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AIGuard.Broker;
using AIGuard.IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Publishing;
using Newtonsoft.Json;

namespace AIGuard.PresenceDetector
{
    public class Worker : BackgroundService
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        private readonly ILogger<Worker> _logger;
        private readonly IDictionary<string, string> _ipRange;
        private readonly IPublishDetections<MqttClientPublishResult> _publisher;
        private readonly int _checkFrequency;
        private readonly Dictionary<string, WatchedObject> _watched;
        private Dictionary<string, List<bool>> _hits;

        private const string FOUND = "FOUND";
        private const string NOTFOUND = "NOT FOUND";

        public Worker(ILogger<Worker> logger, 
            IDictionary<string, string> IPRange, 
            IPublishDetections<MqttClientPublishResult> publisher, 
            int checkFrequency,
            Dictionary<string,WatchedObject> Watched)
        {
            _logger = logger;
            _ipRange = IPRange;
            _publisher = publisher;
            _checkFrequency = checkFrequency;
            _watched = Watched;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                Object obj = new object();

                ParallelOptions options = new ParallelOptions();
                options.CancellationToken = stoppingToken;

                Parallel.ForEach(_watched, options, (watchedItem) =>
                {
                    IPAddress dst = IPAddress.Parse(watchedItem.Value.IP); // the destination IP address
                    byte[] macAddr = new byte[6];
                    uint macAddrLen = (uint)macAddr.Length;
                    lock (obj)
                    {
                        if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) == 0)
                        {

                            string[] str = new string[(int)macAddrLen];
                            for (int i = 0; i < macAddrLen; i++)
                                str[i] = macAddr[i].ToString("x2");

                            watchedItem.Value.AddDiscovery(true);
                            _logger.LogInformation($"{watchedItem.Key}  {string.Join(":", str)} {watchedItem.Value.FoundValue}");
                        }
                        else
                        {
                            watchedItem.Value.AddDiscovery(false);
                            _logger.LogInformation($"{watchedItem.Key}  {watchedItem.Value.NotFoundValue}");
                        }
                    }
                    if (watchedItem.Value.Avalaible)
                    {
                        _publisher.PublishAsync(FOUND, "JOHN", options.CancellationToken);
                    }
                    else
                    {
                        _publisher.PublishAsync(NOTFOUND, "JOHN", options.CancellationToken);
                    }
                });

                
                await Task.Delay(_checkFrequency, stoppingToken);
            }
        }

        
    }
}
