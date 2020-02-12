using AIGaurd.Broker;
using IRepository;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MqttRepository
{
    public class MqttPublish : IPublish<MqttClientPublishResult>
    {
        private readonly string _server;
        private readonly string _clientName;

        public MqttPublish(string server, string clientName)
        {
            _server = server;
            _clientName = clientName;
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

                //TODO: need a better way to determine topic
                var messageMqtt = new MqttApplicationMessageBuilder()
                    .WithTopic($"AI/{source.Split('.')[0]}/{source}")
                    .WithPayload(JsonSerializer.Serialize<IPrediction>(message))
                    .Build();
                return mqttClient.PublishAsync(messageMqtt, CancellationToken.None);
            }
            
        }
    }
}
