# RabbitAdoption Solution Documentation

## 1. Architecture Overview

The RabbitAdoption solution is structured as a modular .NET system with the following main components:

- RabbitAdoption.Api: ASP.NET Core Web API for managing rabbits and adoption requests, exposing endpoints for CRUD and statistics.
- RabbitAdoption.Worker: .NET Worker Service that processes adoption requests asynchronously from a RabbitMQ queue.
- RabbitAdoption.Infrastructure: Contains data access (EF Core), messaging, and configuration logic.
- RabbitAdoption.Application: Application layer with business logic, commands, queries, and MediatR handlers.
- RabbitAdoption.Domain: Domain entities and business rules.
- RabbitAdoption.Tests: Unit and integration tests.

Key Technologies:
- .NET 8/9, Entity Framework Core (SQLite), RabbitMQ, Redis, Docker Compose, MediatR, Serilog.

Data Flow:
- The API receives adoption requests and publishes messages to RabbitMQ.
- The Worker consumes messages, processes adoptions, and updates the database.
- Redis is used for caching adoption status.
- Both API and Worker share the same SQLite database (mounted as a Docker volume).

## 2. Setup Instructions

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- (Optional) Visual Studio or Rider for development

### Running with Docker Compose

1. Clone the repository:
   git clone <your-repo-url>
   cd RabbitAdoption

2. Start all services:
   docker-compose up --build

   This will start:
   - RabbitMQ (with management UI at http://localhost:15672, user: guest/guest)
   - Redis
   - API (http://localhost:5000, Swagger UI enabled)
   - Worker

**Atention: Sometimes the worker might start before RabbitMq container starts so it will automatically stop, you just need to start the Worker container again (future improvements to be retry to connect to RabbitMq for a period, or gracefully log the error and not break).Also the program reads data from a local and physical database, that is located in the Database folder.**

3. Database:
   - The SQLite database is stored in the Database folder at the solution root and is shared between API and Worker.

### Running Locally (without Docker)

1. Start RabbitMQ and Redis locally (e.g., using Docker):
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   docker run -d --name redis -p 6379:6379 redis:7

2. Update appsettings.json and appsettings.Development.json to use localhost for RabbitMQ and Redis.

3. Run the API and Worker projects from your IDE or with:
   dotnet run --project RabbitAdoption.Api
   dotnet run --project RabbitAdoption.Worker

## 3. Database Schema Explanation

The solution uses a single SQLite database with the following main tables:

- Rabbits
  - Id (GUID, PK)
  - Size (string)
  - Color (string)
  - Age (int)
  - IsAdopted (bool)

- AdoptionRequests
  - Id (GUID, PK)
  - AdopterName (string)
  - Contact (string)
  - Preferences_Color (string)
  - Preferences_Breed (string)
  - Preferences_AgeRange (string)
  - Priority (int)
  - Status (int, enum)
  - CreatedAt (datetime)

Indexes:
- Composite index on Rabbits(Size, Color, Age) for fast matching.
- (Optional) Index on Rabbits(IsAdopted) for statistics.

Seeding:
- On startup, the API seeds the Rabbits table with sample data if empty.

## 4. How to Extend the Solution

With more time, the following improvements and extensions could be made:

- **Message Persistence & Enhanced Dead Letter Queue:**
  - Messages that cannot be processed due to high load will be persisted for later retry, ensuring no data loss.
  - The Dead Letter Queue (DLQ) will be enhanced to support advanced monitoring, reprocessing, and alerting for failed messages.

- **API Rate Limiting:**
  - Rate limiting will be added to the API to prevent abuse and protect resources during peak periods.
  - The built-in .NET 8 middleware (AddRateLimiter) can be used for this purpose.

- **Dynamic Worker Scaling:**
  - Deployment will be improved to allow scaling the number of worker instances based on system load, for better throughput and resilience.

- **Authentication & Authorization:** Add user accounts and secure endpoints.
- **Admin Dashboard:** Build a UI for managing rabbits, adoptions, and statistics.
- **Advanced Matching:** Implement more sophisticated matching algorithms (e.g., by breed, temperament).
- **Notifications:** Integrate email/SMS notifications for adoption status updates.
- **Health Checks & Monitoring:** Add health endpoints and integrate with monitoring tools (Prometheus, Grafana).
- **Scalability:** Move from SQLite to PostgreSQL or SQL Server for production, and use Kubernetes for orchestration.
- **Testing:** Add more comprehensive integration and end-to-end tests.
- **API Versioning & Documentation:** Improve API documentation and support for versioning.
- **CI/CD:** Set up automated build, test, and deployment pipelines.

---

This README provides an overview, setup, database details, and a roadmap for future improvements including message persistence, enhanced DLQ, API rate limiting, and dynamic worker scaling.
