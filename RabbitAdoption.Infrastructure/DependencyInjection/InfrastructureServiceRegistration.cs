using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Infrastructure.Configuration;
using RabbitAdoption.Infrastructure.Messaging;

namespace RabbitAdoption.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));
            services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
            services.AddScoped<IAdoptionRequestRepository, AdoptionRequestRepository>();
            services.AddScoped<IRabbitRepository, RabbitRepository>();

            // Redis connection from config
            var redisConfig = configuration.GetSection("Redis:ConnectionString").Value ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConfig));
            services.AddSingleton<IAdoptionStatusCache, RedisAdoptionStatusCache>();

            return services;
        }
    }
}
