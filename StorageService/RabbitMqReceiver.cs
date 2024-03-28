using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Config;
using System.Text;
using System.Text.Json;

namespace StorageService
{
    public sealed class RabbitMqReceiver<T> : IHostedService, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IMessageStorage<T> _messageStorage;
        private readonly ILogger<RabbitMqReceiver<T>> _logger;

        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqReceiver(
            IOptions<RabbitMqConfig> rabbitMqConfig,
            IMessageStorage<T> messageStorage,
            ILogger<RabbitMqReceiver<T>> logger)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = rabbitMqConfig.Value.Host,
                Port = rabbitMqConfig.Value.Port,
                UserName = rabbitMqConfig.Value.UserName,
                Password = rabbitMqConfig.Value.Password
            };
            _messageStorage = messageStorage;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Service} is starting.", typeof(RabbitMqReceiver<T>).Name);

            if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
                return;

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = $"{typeof(T).Name}Queue";
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += MessageReceived;

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        private void MessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            try
            {
                var body = args.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(jsonMessage);

                _messageStorage.Store(message!);

                _channel?.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error message");
                _channel?.BasicNack(deliveryTag: args.DeliveryTag, multiple: false, requeue: true);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Service} is stopping.", typeof(RabbitMqReceiver<T>).Name);

            _channel?.Close();
            _connection?.Close();

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
