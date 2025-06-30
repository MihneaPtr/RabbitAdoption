using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitAdoption.Application.AdoptionRequests.Queries;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.AdoptionRequests.Handlers
{
    public class GetAdoptionRequestStatusQueryHandler : IRequestHandler<GetAdoptionRequestStatusQuery, string>
    {
        private readonly IAdoptionRequestRepository _repository;
        private readonly IAdoptionStatusCache _cache;
        public GetAdoptionRequestStatusQueryHandler(IAdoptionRequestRepository repository, IAdoptionStatusCache cache)
        {
            _repository = repository;
            _cache = cache;
        }
        public async Task<string> Handle(GetAdoptionRequestStatusQuery request, CancellationToken cancellationToken)
        {
            var cached = await _cache.GetStatusAsync(request.Id);
            if (cached != null)
                return cached;
            var adoptionStatus = await _repository.GetStatusByIdAsync(request.Id, cancellationToken);
            var statusStr = adoptionStatus?.ToString() ?? "Unknown";
            await _cache.SetStatusAsync(request.Id, statusStr);
            return statusStr;
        }
    }
}
