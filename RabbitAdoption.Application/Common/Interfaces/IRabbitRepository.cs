using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Common.Interfaces
{
    public interface IRabbitRepository
    {
        Task AddAsync(Rabbit rabbit, CancellationToken cancellationToken = default);
        Task<Rabbit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Rabbit?> GetBestMatchAsync(string? size, string? color, string? ageRange, CancellationToken cancellationToken = default);
        Task<List<RabbitsAdoptedPerDayDto>> GetRabbitsAdoptedPerDayAsync(int days, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Rabbit, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<Rabbit>> GetAllAsync(CancellationToken cancellationToken = default);
    }

    public class RabbitsAdoptedPerDayDto
    {
        public DateTime Date { get; set; }
        public int AdoptedCount { get; set; }
    }
}
