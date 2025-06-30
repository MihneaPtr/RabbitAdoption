using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Infrastructure
{
    public class AdoptionRequestRepository : IAdoptionRequestRepository
    {
        private readonly RabbitAdoptionDbContext _dbContext;
        public AdoptionRequestRepository(RabbitAdoptionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(AdoptionRequest request, CancellationToken cancellationToken = default)
        {
            await _dbContext.AdoptionRequests.AddAsync(request, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<AdoptionStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.AdoptionRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return entity?.Status;
        }
    }
}
