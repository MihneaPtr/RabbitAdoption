using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Worker
{
    public class MessageProcessorWorker : BackgroundService
    {
        private readonly ILogger<MessageProcessorWorker> _logger;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private IModel _channel;

        public MessageProcessorWorker(ILogger<MessageProcessorWorker> logger, IConnection connection, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();
            _channel.BasicQos(0, 5, false); // max 5 mesaje simultan
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                int maxRetries = 3;
                int retryCount = 0;
                int delayMs = 1000;
                while (retryCount < maxRetries)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var adoptionRepo = scope.ServiceProvider.GetRequiredService<IAdoptionRequestRepository>();
                    var rabbitRepo = scope.ServiceProvider.GetRequiredService<IRabbitRepository>();
                    var dbContext = scope.ServiceProvider.GetService<RabbitAdoption.Infrastructure.RabbitAdoptionDbContext>();
                    using var transaction = dbContext?.Database.BeginTransaction();
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var request = JsonSerializer.Deserialize<RequestMessage>(message);
                        if (request == null)
                            throw new Exception("Invalid message format");

                        var adoptionRequest = dbContext.AdoptionRequests.FirstOrDefault(x => x.Id == request.RequestId);
                        if (adoptionRequest == null)
                            throw new Exception($"AdoptionRequest {request.RequestId} not found");

                        var rabbit = await rabbitRepo.GetBestMatchAsync(
                            adoptionRequest.Preferences.Size,
                            adoptionRequest.Preferences.Color,
                            adoptionRequest.Preferences.AgeRange,
                            CancellationToken.None);

                        if (rabbit != null)
                        {
                            dbContext.Rabbits.Attach(rabbit);
                            rabbit.MarkAsAdopted();
                            adoptionRequest.SetStatus(AdoptionStatus.Approved);
                            dbContext.SaveChanges();
                            transaction?.Commit();
                            Console.WriteLine($"Approved adoption {adoptionRequest.Id} with rabbit {rabbit.Id}");
                            _logger.LogInformation("Approved adoption {AdoptionId} with rabbit {RabbitId}", adoptionRequest.Id, rabbit.Id);
                        }
                        else
                        {
                            adoptionRequest.SetStatus(AdoptionStatus.Rejected);
                            dbContext.SaveChanges();
                            transaction?.Commit();
                            Console.WriteLine($"Rejected adoption {adoptionRequest.Id}");
                            _logger.LogError("Rejected adoption {AdoptionId}", adoptionRequest.Id);
                        }
                        _channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        retryCount++;
                        _logger.LogWarning(ex, $"Retry {retryCount}/{maxRetries} for message {ea.DeliveryTag}");
                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError(ex, "Failed to process message after retries, sending to DLQ");
                            _channel.BasicNack(ea.DeliveryTag, false, false); // send to DLQ
                            return;
                        }
                        await Task.Delay(delayMs * retryCount, stoppingToken); // exponential backoff
                    }
                }
            };
            _channel.BasicConsume(
                queue: "main_processing_queue",
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

    public class RequestMessage
    {
        public Guid RequestId { get; set; }
        public int Priority { get; set; }
    }
}
