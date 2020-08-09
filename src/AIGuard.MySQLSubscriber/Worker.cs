using System;
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

        public Worker(ILogger<Worker> logger, string MySQLConnectionString, string MqttServer)
        {
            _logger = logger;
            _mySQLConnectionString = MySQLConnectionString;
            _mqttServer = MqttServer;
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
                var payload = JsonSerializer.Deserialize<Capture>(e.ApplicationMessage.Payload);
                if (payload != null)
                {
                    _logger.LogInformation($"Message recieved {payload.FileName}");
                    using (AIContext context = new AIContext(_mySQLConnectionString)) 
                    {
                        context.Captures.Add(payload);
                        context.SaveChanges();
                    }
                }
            });

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async e =>
            {
                _logger.LogInformation("### CONNECTED WITH SERVER ###");
                await client.SubscribeAsync(new MqttTopicFilter { Topic = "AI/#", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                _logger.LogInformation("### SUBSCRIBED ###");
            });

            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async e =>
            {
                _logger.LogInformation("### DISCONNECTED FROM SERVER ###");
                try
                {
                    await client.ConnectAsync(clientOptions);
                }
                catch
                {
                    _logger.LogError("### RECONNECTING FAILED ###");
                }
            });

            try
            {
                await client.ConnectAsync(clientOptions);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("### CONNECTING FAILED ###" + Environment.NewLine + exception);
            }

            _logger.LogInformation("### WAITING FOR APPLICATION MESSAGES ###");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
