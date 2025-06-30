using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitAdoption.Domain;
using System;
using System.Linq;

namespace RabbitAdoption.Infrastructure
{
    public static class DbSeeder
    {
        public static void SeedRabbits(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RabbitAdoptionDbContext>();
            if (!context.Rabbits.Any())
            {
                context.Rabbits.AddRange(
                    new Rabbit(Guid.NewGuid(), "Small", "White", 1),
                    new Rabbit(Guid.NewGuid(), "Medium", "Brown", 2),
                    new Rabbit(Guid.NewGuid(), "Large", "Black", 3),
                    new Rabbit(Guid.NewGuid(), "Small", "Gray", 2),
                    new Rabbit(Guid.NewGuid(), "Medium", "White", 4)
                );
                context.SaveChanges();
            }
        }
    }
}
