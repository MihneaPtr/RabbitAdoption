using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitAdoption.Worker
{
    public class DeadLetterQueueWorker : BackgroundService
    {
        private readonly ILogger<DeadLetterQueueWorker> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public DeadLetterQueueWorker(ILogger<DeadLetterQueueWorker> logger, IConnection connection, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogError("[DLQ] Message received in dead_letter_queue: {Message}", message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(
                queue: "dead_letter_queue",
                autoAck: false,
                consumer: consumer
            );
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
