using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RabbitAdoption.Infrastructure
{
    public class RabbitRepository : IRabbitRepository
    {
        private readonly RabbitAdoptionDbContext _dbContext;
        public RabbitRepository(RabbitAdoptionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Rabbit rabbit, CancellationToken cancellationToken = default)
        {
            await _dbContext.Rabbits.AddAsync(rabbit, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Rabbit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rabbits.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Rabbit?> GetBestMatchAsync(string? size, string? color, string? ageRange, CancellationToken cancellationToken = default)
        {
            int? age = null;
            if (int.TryParse(ageRange, out var parsedAge))
                age = parsedAge;
            var query = _dbContext.Rabbits.AsQueryable();
            query = query.Where(r => !r.IsAdopted);
            if (!string.IsNullOrEmpty(size))
                query = query.Where(r => r.Size.ToLower() == size.ToLower());
            if (!string.IsNullOrEmpty(color))
                query = query.Where(r => r.Color.ToLower() == color.ToLower());
            if (age != null)
                query = query.Where(r => r.Age == age);
            return await query.OrderBy(r => r.Age).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Rabbit, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rabbits.CountAsync(predicate, cancellationToken);
        }

        public async Task<List<RabbitsAdoptedPerDayDto>> GetRabbitsAdoptedPerDayAsync(int days, CancellationToken cancellationToken = default)
        {
            
            var sql = @"
                WITH Adopted AS (
                    SELECT date(""CreatedAt"") as Day, COUNT(*) as AdoptedCount
                    FROM AdoptionRequests
                    WHERE Status = 1 -- Approved
                      AND date(""CreatedAt"") >= date('now', @days)
                    GROUP BY date(""CreatedAt"")
                )
                SELECT Day as Date, AdoptedCount
                FROM Adopted
                ORDER BY Day DESC
            ";
            var daysParam = new SqliteParameter("@days", $"-{days} days");
            var result = new List<RabbitsAdoptedPerDayDto>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(daysParam);
                _dbContext.Database.OpenConnection();
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        result.Add(new RabbitsAdoptedPerDayDto
                        {
                            Date = DateTime.Parse(reader["Date"].ToString()),
                            AdoptedCount = Convert.ToInt32(reader["AdoptedCount"])
                        });
                    }
                }
            }
            return result;
        }

        public async Task<List<Rabbit>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Rabbits.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
