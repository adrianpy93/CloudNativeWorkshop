# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

This is a .NET 9 cloud-native workshop project using .NET Aspire for orchestration and cloud-native development patterns. The solution consists of:

- **Dometrain.Monolith.Api**: Main API project containing domain modules (Courses, Students, Orders, ShoppingCarts, Enrollments, Identity)
- **Dometrain.Aspire.AppHost**: .NET Aspire orchestration host that manages application services and dependencies
- **Dometrain.Aspire.ServiceDefaults**: Shared service configuration including telemetry, resilience, and service discovery

### Infrastructure Components
- **PostgreSQL**: Primary database (port 5433) for transactional data
- **Azure Cosmos DB**: Document store for shopping carts (runs as emulator in development with Data Explorer)
- **Redis**: Distributed cache for Courses and ShoppingCarts (includes RedisInsight management UI)
- **RabbitMQ**: Message broker (includes management plugin)

### Domain Structure
The main API follows a modular monolith pattern with these domains:
- Identity (JWT authentication, admin authorization)
- Students (user management with password hashing)
- Courses (course catalog)
- ShoppingCarts (Cosmos DB-based cart storage)
- Orders (purchase processing)
- Enrollments (course enrollment management)

## Development Commands

### Build and Run
```bash
# Build the entire solution
dotnet build CloudNativeWorkshop.sln

# Run via Aspire AppHost (recommended - starts all services)
dotnet run --project src/Dometrain.Aspire.AppHost

# Run API directly (for debugging)
dotnet run --project src/Dometrain.Monolith.Api
```

### Database
```bash
# Start PostgreSQL via Docker Compose
docker compose up db

# Database initialization happens automatically on startup via DbInitializer
```

### Testing
Use the provided Insomnia collection (`insomnia-latest.yaml`) or the JSON version (`Insomnia_collection.json`) for API testing.

## Configuration

### Required Parameters (AppHost)
- `postgres-username`: PostgreSQL username
- `postgres-password`: PostgreSQL password
- `cosmosdb-account`: Cosmos DB account (production only)
- `cosmosdb-rg`: Cosmos DB resource group (production only)

### Key Settings
- Database connection: Named "dometrain" in Aspire
- Cosmos DB connection: Named "carts" in Aspire
- JWT settings configured in `Identity:Key`, `Identity:Issuer`, `Identity:Audience`
- Admin API key in `Identity:AdminApiKey`

## Development Notes

### Authentication
- Uses JWT Bearer tokens with symmetric key validation
- Admin policy requires specific claim (`is_admin: true`)
- ApiAdmin policy uses API key authentication
- Password hashing via ASP.NET Core Identity

### Error Handling
Centralized exception handling via `ProblemExceptionHandler` with problem details support.

### Validation
FluentValidation is configured for request validation across all endpoints.

### Database Patterns
- Dapper for data access with `IDbConnectionFactory`
- Repository pattern for data access layers
- Singleton lifetime for repositories and services
- Decorator pattern for caching: `CachedCourseRepository` and `CachedShoppingCartRepository` wrap base repositories and add Redis caching layer

### Observability
- OpenTelemetry configured via ServiceDefaults (metrics, traces, logs)
- Prometheus for metrics collection (`prometheus/prometheus.yml`)
- Grafana for visualization (`grafana/config/`)
- Built-in health checks and telemetry endpoints via Aspire

## Important Notes

- **Always run via Aspire AppHost** (`dotnet run --project src/Dometrain.Aspire.AppHost`) - this orchestrates all infrastructure dependencies
- Default admin user is created on startup: `admin@dometrain.com` with ID `005d25b1-bfc8-4391-b349-6cec00d1416c`
- JWT lifetime is configurable via `Identity:Lifetime` setting (default: 8 hours)