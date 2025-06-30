using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Infrastructure
{
    public class AdoptionRequestConfiguration : IEntityTypeConfiguration<AdoptionRequest>
    {
        public void Configure(EntityTypeBuilder<AdoptionRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.OwnsOne(x => x.Preferences);
            builder.Property(x => x.YearsRabbitExperience);
        }
    }
}
