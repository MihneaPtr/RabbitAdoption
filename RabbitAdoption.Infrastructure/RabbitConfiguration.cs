using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Infrastructure
{
    public class RabbitConfiguration : IEntityTypeConfiguration<Rabbit>
    {
        public void Configure(EntityTypeBuilder<Rabbit> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Size);
            builder.Property(r => r.Color);
            builder.Property(r => r.Age);
            builder.Property(r => r.IsAdopted);
            builder.HasIndex(r => new { r.Size, r.Color, r.Age });
        }
    }
}
