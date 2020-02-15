using AIGaurd.Broker;
using IRepository;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MqttRepository
{
    public class MqttPublish : IPublish<MqttClientPublishResult>
    {
        private readonly string _server;
        private readonly string _clientName;
        private readonly string _regexPattern;
        private readonly int _position;
        private readonly string _queueName;

        public MqttPublish(string server, string clientName, string regexPattern, int position, string queueName)
        {
            //Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(server),
            //                                         "MqttPublish:server cannot be null");
            //Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(queueName),
            //    "MqttPublish:queueName cannot be null");
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
                return mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                        .WithTopic($"{_queueName}/{topicName[_position]}")
                                        .WithPayload(JsonSerializer.Serialize(message))
                                        .Build(), 
                                 CancellationToken.None);
            }
            
        }
    }
}
