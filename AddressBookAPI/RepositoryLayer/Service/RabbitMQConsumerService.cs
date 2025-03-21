using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Service
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitMQConsumerService(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

            var rabbitMQConfig = configuration.GetSection("RabbitMQ");

            _factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfig["Host"],
                UserName = rabbitMQConfig["Username"],
                Password = rabbitMQConfig["Password"]
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _queueName = rabbitMQConfig["QueueName"] ?? "UserRegisteredQueue"; // Default queue name

            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[x] Received: {message}");

                // TODO: Add logic to process the event, like sending an email
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
