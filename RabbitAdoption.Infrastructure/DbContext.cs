using Microsoft.EntityFrameworkCore;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Infrastructure
{
    public class RabbitAdoptionDbContext : DbContext
    {
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }

        public RabbitAdoptionDbContext(DbContextOptions<RabbitAdoptionDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdoptionRequestConfiguration());
            modelBuilder.ApplyConfiguration(new RabbitConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
