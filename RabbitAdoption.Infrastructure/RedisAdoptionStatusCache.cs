using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using RabbitAdoption.Application.Common.Interfaces;

namespace RabbitAdoption.Infrastructure
{
    public class RedisAdoptionStatusCache : IAdoptionStatusCache
    {
        private readonly IDatabase _db;
        public RedisAdoptionStatusCache(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        public async Task<string?> GetStatusAsync(Guid requestId)
        {
            var value = await _db.StringGetAsync(GetKey(requestId));
            return value.HasValue ? value.ToString() : null;
        }
        public async Task SetStatusAsync(Guid requestId, string status)
        {
            await _db.StringSetAsync(GetKey(requestId), status, TimeSpan.FromMinutes(10));
        }
        private static string GetKey(Guid id) => $"adoption_status:{id}";
    }
}
