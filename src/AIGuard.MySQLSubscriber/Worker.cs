using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIGuard.MySQLRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;


namespace AIGuard.MySQLSubscriber
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _mySQLConnectionString;
        private readonly string _mqttServer;
        private readonly Stopwatch _stopwatch;

        public Worker(ILogger<Worker> logger, string MySQLConnectionString, string MqttServer)
        {
            _logger = logger;
            _mySQLConnectionString = MySQLConnectionString;
            _mqttServer = MqttServer;
            _stopwatch = new Stopwatch();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var clientOptions = new MqttClientOptions
            {
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = _mqttServer
                }
            };
            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
            {
                if (e.ApplicationMessage.Topic.Contains("False"))
                {
                    _logger.LogInformation($"Message received on False queue");
                }
                else
                {
                    var payload = JsonSerializer.Deserialize<Capture>(e.ApplicationMessage.Payload);
                    if (payload != null)
                    {
                        try
                        {
                            _stopwatch.Start();
                            _logger.LogInformation($"Message recieved {payload.FileName}");
                            using (AIContext context = new AIContext(_mySQLConnectionString))
                            {
                                payload.dt = DateTime.Now;
                                context.Captures.Add(payload);
                                context.SaveChanges();
                            }
                            _stopwatch.Stop();
                            _logger.LogInformation($"Message {payload.FileName} persited in {_stopwatch.ElapsedMilliseconds}ms");
                        }
                        finally
                        {
                            _stopwatch.Reset();
                        }
                    }
                }
            });

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async e =>
            {
                _logger.LogInformation($"Connected to Mqtt");
                await client.SubscribeAsync(new MqttTopicFilter { Topic = "AI/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                _logger.LogInformation($"Subscribed to Mqtt");
            });

            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async e =>
            { 
                _logger.LogInformation($"Disconnected from server");
                try
                {
                    await client.ConnectAsync(clientOptions);
                }
                catch (Exception x)
                {
                    _logger.LogError($"Reconnection failed {x.Message}");
                }
            });

            try
            {
                await client.ConnectAsync(clientOptions);
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Connection Failed {exception?.Message}");
            }

            _logger.LogInformation("Waiting for messages.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
