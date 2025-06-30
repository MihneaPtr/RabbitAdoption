using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitAdoption.Infrastructure;
using RabbitAdoption.Infrastructure.DependencyInjection;
using MediatR;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy (allow all for dev, restrict in prod)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Use environment variable for DB path in Docker, fallback to solution-relative path for local
var dbPathFromEnv = Environment.GetEnvironmentVariable("RABBIT_DB_PATH");
string dbFullPath;
if (!string.IsNullOrEmpty(dbPathFromEnv))
{
    dbFullPath = dbPathFromEnv;
    Directory.CreateDirectory(Path.GetDirectoryName(dbFullPath)!);
}
else
{
    var solutionDir = Directory.GetParent(AppContext.BaseDirectory)!;
    while (solutionDir != null && !File.Exists(Path.Combine(solutionDir.FullName, "RabbitAdoption.sln")))
        solutionDir = solutionDir.Parent;
    if (solutionDir == null)
        throw new DirectoryNotFoundException("Could not find solution directory containing RabbitAdoption.sln");
    var dbFolder = Path.Combine(solutionDir.FullName, "Database");
    dbFullPath = Path.Combine(dbFolder, "rabbitadoption.db");
    Directory.CreateDirectory(dbFolder);
}
builder.Services.AddDbContext<RabbitAdoptionDbContext>(options =>
    options.UseSqlite($"Data Source={dbFullPath}"));

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RabbitAdoption.Application.AdoptionRequests.Commands.SubmitAdoptionRequestCommand>());

var app = builder.Build();

// Ensure database and tables exist, and run migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RabbitAdoptionDbContext>();
    dbContext.Database.Migrate(); 
    DbSeeder.SeedRabbits(scope.ServiceProvider); // seed pentru tabela Rabbits
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
