using MediatR;
using RabbitAdoption.Application.AdoptionRequests.Commands;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitAdoption.Application.AdoptionRequests.Handlers
{
    public class SubmitAdoptionRequestCommandHandler : IRequestHandler<SubmitAdoptionRequestCommand, Guid>
    {
        private readonly IAdoptionRequestRepository _repository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public SubmitAdoptionRequestCommandHandler(IAdoptionRequestRepository repository, IRabbitMqPublisher rabbitMqPublisher)
        {
            _repository = repository;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<Guid> Handle(SubmitAdoptionRequestCommand request, CancellationToken cancellationToken)
        {
            var preferences = new RabbitPreferences(request.Color, string.Empty, request.Age?.ToString() ?? string.Empty, request.Size);
            var adoptionRequest = new AdoptionRequest(Guid.NewGuid(), request.AdopterName, request.Contact, preferences, request.Priority, request.YearsRabbitExperience);
            await _repository.AddAsync(adoptionRequest, cancellationToken);

            var priorityScore = (int)adoptionRequest.Priority + request.YearsRabbitExperience;
            var cappedPriority = Math.Min(priorityScore, 10); // max 10 pentru RabbitMQ

            var queueMessage = new
            {
                RequestId = adoptionRequest.Id,
                Priority = cappedPriority
            };
            await _rabbitMqPublisher.PublishAsync("main_processing_queue", queueMessage, cappedPriority);

            return adoptionRequest.Id;
        }
    }
}
