using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Infrastructure.Configuration;
using RabbitMQ.Client;

namespace RabbitAdoption.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly RabbitMQ.Client.IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;

            var factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishAsync(string queueName, object message, int priority = 0)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Priority = (byte)priority;
                _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new RabbitMqPublishException("Failed to publish message to RabbitMQ", ex);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    public class RabbitMqPublishException : Exception
    {
        public RabbitMqPublishException(string message, Exception inner) : base(message, inner) { }
    }
}
