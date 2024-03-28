using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Config;
using System.Text;
using System.Text.Json;

namespace PixelService
{
    public sealed class RabbitMqMessageSender : IMessageSender
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMqMessageSender(IOptions<RabbitMqConfig> rabbitMqConfig)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = rabbitMqConfig.Value.Host,
                Port = rabbitMqConfig.Value.Port,
                UserName = rabbitMqConfig.Value.UserName,
                Password = rabbitMqConfig.Value.Password
            };
        }

        public void SendMessage<T>(T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = $"{typeof(T).Name}Queue";
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
    }
}
