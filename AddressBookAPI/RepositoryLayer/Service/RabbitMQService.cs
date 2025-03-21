using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQService(IConfiguration configuration)
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
        }

        public void PublishMessage(string queueName, string message)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                Console.WriteLine($"[x] Sent message: {message} to queue: {queueName}");
            }
        }
    }
}
