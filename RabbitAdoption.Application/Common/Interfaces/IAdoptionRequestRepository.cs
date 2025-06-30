using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Common.Interfaces
{
    public interface IAdoptionRequestRepository
    {
        Task AddAsync(AdoptionRequest request, CancellationToken cancellationToken = default);
        Task<AdoptionStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
