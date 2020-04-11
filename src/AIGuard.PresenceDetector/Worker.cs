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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AIGuard.PresenceDetector
{
    public class Worker : BackgroundService
    {

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                Object obj = new object();

                RangeFinder range = new RangeFinder();
                IPAddress startIP = IPAddress.Parse("192.168.0.240");
                IPAddress endIP = IPAddress.Parse("192.168.0.240");
                IEnumerable<string> ipAddresses = range.GetIPRange(startIP, endIP);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                CancellationTokenSource cts = new CancellationTokenSource();
                //cts.CancelAfter(300);

                ParallelOptions options = new ParallelOptions();
                options.CancellationToken = cts.Token;

                Parallel.ForEach(ipAddresses, options, (ipAddress) =>
                {

                    IPAddress dst = IPAddress.Parse(ipAddress); // the destination IP address

                    // Ping ping = new Ping();
                    // PingReply pingReply = ping.Send(dst, 300);
                    // if (pingReply.Status == IPStatus.Success)
                    // {

                    byte[] macAddr = new byte[6];
                    uint macAddrLen = (uint)macAddr.Length;

                    lock (obj)
                    {
                        if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) == 0)
                        {

                            string[] str = new string[(int)macAddrLen];
                            for (int i = 0; i < macAddrLen; i++)
                                str[i] = macAddr[i].ToString("x2");

                            _logger.LogInformation($"{ipAddress}  {string.Join(":", str)} FOUND");
                        }
                        else
                        {
                            _logger.LogInformation($"{ipAddress}  NOT Found");
                        }
                    }

                    options.CancellationToken.ThrowIfCancellationRequested();
                    //}
                });

                sw.Stop();
                _logger.LogInformation(sw.Elapsed.ToString());


                await Task.Delay(15000, stoppingToken);

            }
        }

        public class RangeFinder
        {
            public IEnumerable<string> GetIPRange(IPAddress startIP,
                IPAddress endIP)
            {
                uint sIP = ipToUint(startIP.GetAddressBytes());
                uint eIP = ipToUint(endIP.GetAddressBytes());
                while (sIP <= eIP)
                {
                    yield return new IPAddress(reverseBytesArray(sIP)).ToString();
                    sIP++;
                }
            }


            /* reverse byte order in array */
            protected uint reverseBytesArray(uint ip)
            {
                byte[] bytes = BitConverter.GetBytes(ip);
                bytes = bytes.Reverse().ToArray();
                return (uint)BitConverter.ToInt32(bytes, 0);
            }


            /* Convert bytes array to 32 bit long value */
            protected uint ipToUint(byte[] ipBytes)
            {
                ByteConverter bConvert = new ByteConverter();
                uint ipUint = 0;

                int shift = 24; // indicates number of bits left for shifting
                foreach (byte b in ipBytes)
                {
                    if (ipUint == 0)
                    {
                        ipUint = (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                        shift -= 8;
                        continue;
                    }

                    if (shift >= 8)
                        ipUint += (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                    else
                        ipUint += (uint)bConvert.ConvertTo(b, typeof(uint));

                    shift -= 8;
                }

                return ipUint;
            }
        }
    }
}
