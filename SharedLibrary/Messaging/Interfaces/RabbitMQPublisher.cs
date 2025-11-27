using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Messaging.Interfaces
{

    public class RabbitMQPublisher : SharedLibrary.Messaging.Implementations.IRabbitMQPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQPublisher(
            string host = "localhost",
            int port = 5672,
            string user = "guest",
            string password = "guest")
        {
            _factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = user,
                Password = password
            };
        }

        public Task PublishAsync(string queueName, object message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: body);

            return Task.CompletedTask;
        }
    }
}