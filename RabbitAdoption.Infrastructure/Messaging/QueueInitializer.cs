using RabbitMQ.Client;
using RabbitAdoption.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace RabbitAdoption.Infrastructure.Messaging
{
    public class QueueInitializer
    {
        private readonly RabbitMqSettings _settings;

        public QueueInitializer(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public void InitializeQueues()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };
            Console.WriteLine($"[QueueInitializer] RabbitMQ config: HostName={factory.HostName}, Port={factory.Port}, UserName={factory.UserName}, Password={factory.Password}");
            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();


                var mainQueueArgs = new Dictionary<string, object>
                {
                    { "x-max-priority", 10 },
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key", "dead_letter_queue" },
                    { "x-max-length", 10000 },
                    { "x-message-ttl", 2 * 24 * 60 * 60 * 1000 }
                };
                channel.QueueDeclare(
                    queue: "main_processing_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: mainQueueArgs
                );
                channel.QueueDeclare(
                    queue: "dead_letter_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QueueInitializer] Failed to connect to RabbitMQ: {ex.Message}");
                throw;
            }
        }
    }
}
