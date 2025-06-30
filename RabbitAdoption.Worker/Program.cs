using RabbitAdoption.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitAdoption.Worker;
using RabbitAdoption.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitAdoption.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Serilog;

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

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.SQLite(dbFullPath)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Register RabbitMQ ConnectionFactory and IConnection
        services.AddSingleton<IConnection>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var rabbitSettings = configuration.GetSection("RabbitMqSettings").Get<RabbitAdoption.Infrastructure.Configuration.RabbitMqSettings>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitSettings?.HostName ?? "localhost",
                Port = rabbitSettings?.Port ?? 5672,
                UserName = rabbitSettings?.UserName ?? "guest",
                Password = rabbitSettings?.Password ?? "guest"
            };
            return factory.CreateConnection();
        });
        // Register QueueInitializer
        services.AddSingleton<QueueInitializer>();
        // Use relative path for SQLite DB
        services.AddDbContext<RabbitAdoptionDbContext>(options =>
            options.UseSqlite($"Data Source={dbFullPath}"));
        // Register infrastructure/data services
        services.AddInfrastructureServices(context.Configuration);
        // Register the background worker
        services.AddHostedService<MessageProcessorWorker>();
        services.AddHostedService<DeadLetterQueueWorker>();
    })
    .Build();

// Ensure database and tables exist, and run migrations, then call QueueInitializer
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RabbitAdoptionDbContext>();
    dbContext.Database.Migrate(); // ruleaz? automat toate migr?rile EF Core
    var queueInitializer = scope.ServiceProvider.GetRequiredService<QueueInitializer>();
    queueInitializer.InitializeQueues();
}

host.Run();
