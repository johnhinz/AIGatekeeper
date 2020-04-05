using AIGuard.Broker;
using AIGuard.IRepository;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AIGuard.MqttRepository
{
    public class MqttPublish : IPublishDetections<MqttClientPublishResult>
    {
        private readonly string _server;
        private readonly string _clientName;
        private readonly string _regexPattern;
        private readonly int _position;
        private readonly string _queueName;

        public MqttPublish(string server, string clientName, string regexPattern, int position, string queueName)
        {
            if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("MqttPublish:server cannot be null");
            if (string.IsNullOrEmpty(regexPattern)) throw new ArgumentNullException("MqttPublish:regexPattern cannot be null");
            if (position < 0) throw new ArgumentOutOfRangeException("MqttPublish:position < 0");

            _server = server;
            _clientName = clientName;
            _regexPattern = regexPattern;
            _position = position;
            _queueName = queueName;
        }
        public Task<MqttClientPublishResult> PublishAsync(IPrediction message, string source, CancellationToken token) 
        {
            var factory = new MqttFactory();
            using (var mqttClient = factory.CreateMqttClient())
            {
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(_clientName)
                    .WithTcpServer(_server)
                    .WithCleanSession()
                    .Build();

                mqttClient.ConnectAsync(options, CancellationToken.None).Wait();

                string[] topicName = Regex.Split(source, _regexPattern);
                if (!(topicName.Length > _position))
                {
                    throw new ArgumentOutOfRangeException("Sub-topic name cannot be determined.");
                }
                return mqttClient.PublishAsync(
                                new MqttApplicationMessageBuilder()
                                        .WithTopic($"{_queueName}/{topicName[_position]}")
                                        .WithPayload(JsonSerializer.Serialize(message))
                                        .Build(), 
                                 CancellationToken.None);
            }
            
        }
    }
}
