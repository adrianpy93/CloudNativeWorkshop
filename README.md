# Cloud-Native Workshop - NDC Copenhagen 2025

A comprehensive .NET cloud-native workshop demonstrating modern distributed application patterns using .NET Aspire 13 and a modular monolith architecture.

## Overview

This project showcases a complete e-learning platform built with cloud-native principles, featuring:

- **Modular Monolith Architecture** - Domain-driven design with clear bounded contexts
- **.NET Aspire Orchestration** - Streamlined local development with automatic infrastructure provisioning
- **Polyglot Persistence** - PostgreSQL for relational data, Cosmos DB for shopping carts
- **Distributed Caching** - Redis for performance optimization
- **Message-Driven Communication** - RabbitMQ for event-driven patterns
- **Cloud-Ready Design** - Built for containerization and cloud deployment

## Architecture

### Application Structure

```
├── Dometrain.Aspire.AppHost       # Aspire orchestration host
├── Dometrain.Aspire.ServiceDefaults # Shared service configuration
└── Dometrain.Monolith.Api         # Main API (Modular Monolith)
    ├── Courses/                    # Course catalog domain
    ├── Students/                   # Student management domain
    ├── Orders/                     # Order processing domain
    ├── ShoppingCarts/              # Shopping cart domain (Cosmos DB)
    ├── Enrollments/                # Course enrollment domain
    └── Identity/                   # Authentication & authorization
```

### Technology Stack

- **.NET 10** - Latest .NET runtime and SDK
- **.NET Aspire 13** - Cloud-native orchestration and tooling
- **ASP.NET Core Minimal APIs** - Lightweight endpoint definitions
- **PostgreSQL 17** - Primary relational database
- **Azure Cosmos DB** - NoSQL document store for shopping carts
- **Redis** - Distributed caching layer
- **RabbitMQ** - Message broker for async communication
- **Dapper** - Micro-ORM for data access
- **FluentValidation** - Request validation framework
- **OpenTelemetry** - Observability (metrics, traces, logs)
- **Swashbuckle** - OpenAPI/Swagger documentation

### Infrastructure Components

All infrastructure is automatically managed by Aspire:

| Component | Purpose | Port | UI |
|-----------|---------|------|-----|
| PostgreSQL | Transactional data | 5433 | - |
| Cosmos DB Emulator | Shopping carts | - | Data Explorer UI |
| Redis | Caching | - | RedisInsight UI |
| RabbitMQ | Message broker | - | Management Plugin UI |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- IDE: [Visual Studio 2025](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd cloud-native-workshop-ndccph-2025
```

### 2. Build the Solution

```bash
dotnet build CloudNativeWorkshop.sln
```

### 3. Run with Aspire

The recommended way to run the application is through the Aspire AppHost, which automatically starts all infrastructure:

```bash
dotnet run --project src/Dometrain.Aspire.AppHost
```

This will:
- Start PostgreSQL container and create the `dometrain` database
- Launch Cosmos DB emulator with Data Explorer
- Start Redis with RedisInsight
- Launch RabbitMQ with Management Plugin
- Start the main API
- Open the Aspire Dashboard

### 4. Access the Application

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Aspire Dashboard**: Automatically opens in browser

### First-Time Setup

On the first run, the application will:
1. Create the PostgreSQL `dometrain` database
2. Initialize all database tables
3. Create a default admin user:
   - Email: `admin@dometrain.com`
   - ID: `005d25b1-bfc8-4391-b349-6cec00d1416c`

## Project Structure

### Domain Organization

Each domain follows a consistent structure:

```
Domain/
├── {Domain}.cs                    # Domain models
├── {Domain}Repository.cs          # Data access layer
├── Cached{Domain}Repository.cs    # Caching decorator
├── {Domain}Service.cs             # Business logic
├── {Domain}Endpoints.cs           # API endpoint handlers
├── {Domain}EndpointExtensions.cs  # Route registration
├── {Domain}Requests.cs            # Request DTOs
└── {Domain}Validator.cs           # FluentValidation rules
```

### Key Patterns

- **Minimal APIs** - Lightweight endpoint definitions without controllers
- **Repository Pattern** - Abstraction over data access
- **Decorator Pattern** - Caching repositories wrap base repositories
- **Dependency Injection** - Singleton lifetime for most services
- **Request/Response DTOs** - Clear API contracts

## Authentication & Authorization

### JWT Authentication

The API uses JWT Bearer token authentication:

```bash
# Login to get token
POST /auth/login
{
  "email": "admin@dometrain.com",
  "password": "YourPassword"
}
```

### Authorization Policies

- **Admin** - Requires JWT with `is_admin: true` claim
- **ApiAdmin** - Uses API key from configuration

## API Testing

Use the included Insomnia collection for testing:

```bash
# Import the collection
insomnia-latest.yaml
```

The collection includes:
- Authentication flows
- Student management
- Course operations
- Shopping cart interactions
- Order processing
- Enrollment management

## Development

### Running Individual Components

```bash
# Run API directly (requires infrastructure running)
dotnet run --project src/Dometrain.Monolith.Api

# Run with Docker Compose (PostgreSQL only)
docker compose up db
```

### Database Migrations

The application uses code-first migrations via `DbInitializer`:

- Schema creation happens automatically on startup
- Tables are created with `CREATE TABLE IF NOT EXISTS`
- No manual migration steps required

### Adding a New Domain

1. Create domain folder under `Dometrain.Monolith.Api/`
2. Add models, repository, service, endpoints
3. Register endpoints in `Program.cs`
4. Add validation rules
5. Update database initialization if needed

## Configuration

### AppHost Settings

Located in `src/Dometrain.Aspire.AppHost/appsettings.json`:

- Infrastructure connection strings (auto-generated)
- Service discovery configuration
- Telemetry settings

### API Settings

Located in `src/Dometrain.Monolith.Api/appsettings.json`:

```json
{
  "Identity": {
    "Key": "your-secret-key",
    "Issuer": "Dometrain",
    "Audience": "Dometrain.Api",
    "Lifetime": "08:00:00",
    "AdminApiKey": "your-admin-key"
  }
}
```

## Observability

The application includes comprehensive observability via OpenTelemetry:

### Metrics
- Request counts and duration
- Database query performance
- Cache hit/miss ratios
- Message processing metrics

### Traces
- Distributed tracing across services
- Database query tracing
- HTTP request tracing

### Logs
- Structured logging with correlation IDs
- Integration with Aspire Dashboard

### Monitoring Stack

```bash
# Prometheus metrics
http://localhost:9090

# Grafana dashboards
http://localhost:3000

# Aspire Dashboard (built-in)
# Opens automatically on startup
```

## Troubleshooting

### PostgreSQL Connection Issues

If the database fails to connect:

```bash
# Clean up existing containers and volumes
docker stop $(docker ps -q --filter name=main-db) 2>/dev/null || true
docker rm $(docker ps -aq --filter name=main-db) 2>/dev/null || true
docker volume rm $(docker volume ls -q --filter name=main-db) 2>/dev/null || true

# Restart Aspire
dotnet run --project src/Dometrain.Aspire.AppHost
```

### Build Errors

If you encounter build errors:

```bash
# Clean and rebuild
dotnet clean
dotnet build --no-incremental
```

### Port Conflicts

If ports are already in use, update them in `AppHost.cs`:

```csharp
var postgres = builder.AddPostgres("main-db", port: 5433); // Change port here
```

## Cloud Deployment

### Container Support

Build the API as a container:

```bash
docker build -t dometrain-api -f src/Dometrain.Monolith.Api/Dockerfile .
```

### Kubernetes Manifests

Pre-generated Kubernetes manifests are available in:

```
src/Dometrain.Aspire.AppHost/aspirate-output/
```

Deploy to Kubernetes:

```bash
kubectl apply -k src/Dometrain.Aspire.AppHost/aspirate-output/
```

## Design Patterns Demonstrated

### Architectural Patterns
- **Modular Monolith** - Domain separation without microservices complexity
- **Repository Pattern** - Data access abstraction
- **Decorator Pattern** - Caching layer implementation
- **Dependency Injection** - Loose coupling and testability

### Cloud-Native Patterns
- **Health Checks** - Application health monitoring
- **Configuration Management** - External configuration
- **Service Discovery** - Aspire-managed service resolution
- **Distributed Caching** - Redis-based caching
- **Event-Driven Architecture** - RabbitMQ messaging

### Data Patterns
- **Polyglot Persistence** - Different databases for different needs
- **CQRS-lite** - Read/write separation in repositories
- **Optimistic Concurrency** - Version-based conflict detection

## Performance Considerations

### Caching Strategy

The application implements a two-tier caching approach:

1. **L1 Cache** - In-memory application cache
2. **L2 Cache** - Distributed Redis cache

Cached entities:
- Courses (reduces database load)
- Shopping carts (improves response time)

### Database Optimization

- **Connection pooling** - Managed by Npgsql
- **Prepared statements** - Via Dapper
- **Indexed lookups** - On email, slug fields

## Security

### Authentication
- JWT with symmetric key signing
- Configurable token lifetime
- Password hashing via ASP.NET Core Identity

### Authorization
- Claim-based authorization
- API key authentication for admin operations
- Role-based access control

### Best Practices
- Secrets in environment variables (production)
- HTTPS enforcement (production)
- Input validation via FluentValidation
- SQL injection prevention via parameterized queries

## License

This project is for educational purposes as part of the NDC Copenhagen 2025 Cloud-Native Workshop.

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Minimal APIs Guide](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Cloud-Native Patterns](https://www.microsoft.com/en-us/architecture/cloud-native)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)

---

**Workshop**: Cloud-Native Development with .NET Aspire
**Event**: NDC Copenhagen 2025
**Framework**: .NET 10 | Aspire 13
