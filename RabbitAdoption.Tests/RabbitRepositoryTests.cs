using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RabbitAdoption.Domain;
using RabbitAdoption.Infrastructure;
using Xunit;

namespace RabbitAdoption.Tests
{
    public class RabbitRepositoryTests
    {
        private RabbitAdoptionDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<RabbitAdoptionDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RabbitAdoptionDbContext(options);
        }

        [Fact]
        public async Task GetBestMatchAsync_ReturnsPerfectMatch()
        {
            using var db = CreateDbContext();
            db.Rabbits.Add(new Rabbit(Guid.NewGuid(), "Large", "White", 2));
            db.Rabbits.Add(new Rabbit(Guid.NewGuid(), "Small", "Black", 1));
            db.SaveChanges();
            var repo = new RabbitRepository(db);
            var match = await repo.GetBestMatchAsync("Large", "White", "2");
            Assert.NotNull(match);
            Assert.Equal("Large", match.Size);
            Assert.Equal("White", match.Color);
            Assert.Equal(2, match.Age);
        }

        [Fact]
        public async Task GetBestMatchAsync_ReturnsNull_WhenNoMatch()
        {
            using var db = CreateDbContext();
            db.Rabbits.Add(new Rabbit(Guid.NewGuid(), "Large", "White", 2));
            db.SaveChanges();
            var repo = new RabbitRepository(db);
            var match = await repo.GetBestMatchAsync("Small", "Black", "1");
            Assert.Null(match);
        }

        [Fact]
        public async Task GetBestMatchAsync_IgnoresAdopted()
        {
            using var db = CreateDbContext();
            var rabbit = new Rabbit(Guid.NewGuid(), "Large", "White", 2);
            rabbit.MarkAsAdopted();
            db.Rabbits.Add(rabbit);
            db.SaveChanges();
            var repo = new RabbitRepository(db);
            var match = await repo.GetBestMatchAsync("Large", "White", "2");
            Assert.Null(match);
        }
    }
}
