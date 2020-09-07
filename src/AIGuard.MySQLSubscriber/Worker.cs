using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIGuard.IRepository;
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
        private readonly string _mqttServer;
        private readonly IPublishDetections<int> _publisher;
        private readonly IMqttClientFactory _mqttFactory;
        private readonly Stopwatch _stopwatch;

        public Worker(ILogger<Worker> logger, IPublishDetections<int> publisher, IMqttClientFactory mqttFactory, string MqttServer)
        {
            _logger = logger;
            _mqttServer = MqttServer;
            _publisher = publisher;
            _mqttFactory = mqttFactory;
            _stopwatch = new Stopwatch();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var factory = new MqttFactory();
            var client = _mqttFactory.CreateMqttClient();
            var clientOptions = new MqttClientOptions
            {
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = _mqttServer
                }
            };

            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(MessageReceiver());

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(ClientConnect(client));

            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(ClientDisconnect(client, clientOptions));

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

        private Func<MqttClientDisconnectedEventArgs, Task> ClientDisconnect(IMqttClient client, MqttClientOptions clientOptions)
        {
            return async e =>
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
            };
        }

        private Func<MqttClientConnectedEventArgs, Task> ClientConnect(IMqttClient client)
        {
            return async e =>
            {
                _logger.LogInformation($"Connected to Mqtt");
                await client.SubscribeAsync(new MqttTopicFilter { Topic = "AI/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                _logger.LogInformation($"Subscribed to Mqtt");
            };
        }

        private Func<MqttApplicationMessageReceivedEventArgs, Task> MessageReceiver()
        {
            return async e =>
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
                            //IPublishDetections<int> publish = new PublishDetections(_mySQLConnectionString);
                            payload.dt = DateTime.Now;
                            try
                            {
                                await _publisher.PublishAsync<Capture>(payload, string.Empty, CancellationToken.None);
                                foreach (var item in payload.Detections)
                                {
                                    item.CaptureId = payload.Id;
                                    await _publisher.PublishAsync<Detection>(item, string.Empty, CancellationToken.None);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.InnerException?.Message);
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
            };
        }
    }
}
