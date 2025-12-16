# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**CloudNativeWorkshop** is a .NET 10 cloud-native application demonstrating modern patterns with .NET Aspire 13 orchestration. This is a workshop project showcasing modular monolith architecture, polyglot persistence, and contemporary C# 14 features.

- **Repository**: NDC Copenhagen 2025 Workshop
- **Target Framework**: .NET 10
- **Aspire Version**: 13.0.2
- **Architecture**: Modular Monolith

## Solution Structure

```
CloudNativeWorkshop/
├── src/
│   ├── Dometrain.Aspire.AppHost/     # Aspire orchestration host
│   ├── Dometrain.Aspire.ServiceDefaults/  # Shared telemetry, resilience, service discovery
│   └── Dometrain.Monolith.Api/       # Main API (modular monolith)
├── database/                          # SQL initialization scripts
├── grafana/                           # Grafana dashboards config
├── prometheus/                        # Prometheus metrics config
├── docker-compose.yml                 # PostgreSQL standalone config
└── insomnia-latest.yaml              # API testing collection
```

### Projects

| Project | Purpose | SDK |
|---------|---------|-----|
| `Dometrain.Aspire.AppHost` | Orchestrates all infrastructure services | Aspire.AppHost.Sdk/13.0.2 |
| `Dometrain.Aspire.ServiceDefaults` | Shared configuration (OpenTelemetry, resilience, health checks) | Class Library |
| `Dometrain.Monolith.Api` | Main API with 6 domain modules | ASP.NET Core Web API |

## Architecture Overview

### Modular Monolith Pattern

The API follows a **modular monolith** architecture with domain-focused modules in a single deployable unit:

```
Dometrain.Monolith.Api/
├── Identity/         # JWT authentication, authorization
├── Students/         # User management
├── Courses/          # Course catalog
├── ShoppingCarts/    # Cart storage (PostgreSQL)
├── Orders/           # Purchase processing
├── Enrollments/      # Course enrollment
├── Database/         # DbContext, EF Core configurations, migrations
├── ErrorHandling/    # Centralized exception handling
└── OpenApi/          # Swagger configuration
```

### Domain Module Structure

Each domain follows a consistent vertical slice pattern:

```
{Domain}/
├── {Domain}.cs                    # Domain entity
├── {Domain}Endpoints.cs           # Static HTTP handler methods
├── {Domain}EndpointExtensions.cs  # Route registration extension
├── {Domain}Repository.cs          # Data access + interface
├── Cached{Domain}Repository.cs    # Optional caching decorator
├── {Domain}Service.cs             # Business logic + interface
├── {Domain}Mapper.cs              # DTO-to-entity mapping
├── {Domain}Requests.cs            # Request DTOs (records)
└── {Domain}Validator.cs           # FluentValidation rules
```

### Infrastructure Components

All managed by Aspire AppHost (`src/Dometrain.Aspire.AppHost/AppHost.cs`):

| Component | Purpose | Connection Name | Notes |
|-----------|---------|-----------------|-------|
| **PostgreSQL** | Primary database (all domains including shopping carts) | `dometrain` | Port 5433, EF Core |
| **Redis** | Distributed cache | `redis` | Course and cart caching |
| **RabbitMQ** | Message broker | `rabbitmq` | With management plugin |

## Development Commands

### Build and Run

```bash
# Build entire solution
dotnet build CloudNativeWorkshop.sln

# Run via Aspire AppHost (RECOMMENDED - starts all infrastructure)
dotnet run --project src/Dometrain.Aspire.AppHost

# Run API directly (requires infrastructure already running)
dotnet run --project src/Dometrain.Monolith.Api

# Clean and rebuild
dotnet clean && dotnet build --no-incremental
```

### Database

Database initialization happens automatically on startup via EF Core migrations (`context.Database.MigrateAsync()`). The admin user is seeded if not exists.

**EF Core Migrations:**
```bash
# Create a new migration
cd src/Dometrain.Monolith.Api
dotnet ef migrations add MigrationName --output-dir Database/Migrations

# Apply migrations manually (also runs on startup)
dotnet ef database update
```

Standalone PostgreSQL:
```bash
docker compose up db
```

### Testing

Use the provided Insomnia collection (`insomnia-latest.yaml`) for API testing.

## API Endpoints

### Identity
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/identity/login` | Anonymous | Login, returns JWT token |

### Students
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/students` | Anonymous | Register new student |
| GET | `/students/me` | JWT | Get current student |
| GET | `/students/{idOrEmail}` | Admin | Get student by ID/email |
| GET | `/students` | Admin | List all students (paginated) |
| DELETE | `/students/{id:guid}` | Admin | Delete student |

### Courses
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/courses` | Admin | Create course |
| GET | `/courses/{idOrSlug}` | Anonymous | Get by ID or slug |
| GET | `/courses` | Anonymous | List courses (paginated) |
| PUT | `/courses/{id:guid}` | Admin | Update course |
| DELETE | `/courses/{id:guid}` | Admin | Delete course |

### Shopping Cart
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/cart/me` | JWT | Get current cart |
| POST | `/cart/me/courses/{courseId:guid}` | JWT | Add course to cart |
| DELETE | `/cart/me/courses/{courseId:guid}` | JWT | Remove course from cart |
| DELETE | `/cart/me` | JWT | Clear cart |

### Orders
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/orders` | JWT | Get user's orders |
| GET | `/orders/{orderId:guid}` | JWT | Get specific order |
| POST | `/orders` | JWT | Place order |

### Enrollments
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/enrollments` | JWT | Get user's enrollments |
| PUT | `/enrollments/{courseId:guid}` | Admin | Enroll student |
| DELETE | `/enrollments/{courseId:guid}` | Admin | Unenroll student |

### Health (Development only)
- `GET /health` - Readiness check
- `GET /alive` - Liveness check

## Configuration

### Required Aspire Parameters

Configured in `src/Dometrain.Aspire.AppHost/appsettings.json`:

```json
{
  "Parameters": {
    "postgres-username": "workshop",
    "postgres-password": "changeme"
  }
}
```

### Identity Settings

`src/Dometrain.Monolith.Api/appsettings.json`:

| Setting | Description |
|---------|-------------|
| `Identity:Key` | JWT signing key (symmetric) |
| `Identity:Issuer` | Token issuer (https://id.dometrain.com) |
| `Identity:Audience` | Token audience (https://dometrain.com) |
| `Identity:Lifetime` | Token lifetime (default: 08:00:00) |
| `Identity:AdminApiKey` | API key for admin access |

## Design Patterns

### Repository Pattern with Decorator Caching

Base repositories implement data access using EF Core with `IDbContextFactory`, cached decorators wrap them transparently:

```
ICourseRepository (interface)
    ├── CourseRepository (EF Core + IDbContextFactory)
    └── CachedCourseRepository (wraps base + Redis cache)
```

**Repository Pattern** (using `IDbContextFactory` for Singleton repositories):
```csharp
public class CourseRepository(IDbContextFactory<DometrainDbContext> contextFactory) : ICourseRepository
{
    public async Task<Course?> GetByIdAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Courses.FindAsync(id);
    }
}
```

**Registration** (`Program.cs`):
```csharp
builder.Services.AddSingleton<CourseRepository>();
builder.Services.AddSingleton<ICourseRepository>(s =>
    new CachedCourseRepository(
        s.GetRequiredService<CourseRepository>(),
        s.GetRequiredService<IConnectionMultiplexer>()));
```

### Caching Strategy

| Domain | Strategy | Cache Keys |
|--------|----------|------------|
| Courses | Write-through | `course:id:{guid}`, `course:slug:{slug}` |
| ShoppingCarts | Cache invalidation | `cart:id:{studentId}` |

- Read: Cache-aside (check cache first, populate on miss)
- Write: Update DB first, then update/invalidate cache
- No TTL set (persistent until invalidated)

### Minimal API Endpoint Pattern

1. **Handler methods** in `{Domain}Endpoints.cs` - pure static functions
2. **Route registration** in `{Domain}EndpointExtensions.cs` - extension methods

```csharp
// CourseEndpoints.cs
public static class CourseEndpoints
{
    public static async Task<IResult> Get(string idOrSlug, ICourseService courseService)
    {
        var isId = Guid.TryParse(idOrSlug, out var id);
        var course = isId ? await courseService.GetByIdAsync(id) : await courseService.GetBySlugAsync(idOrSlug);
        return course is null ? Results.NotFound() : Results.Ok(course);
    }
}

// CourseEndpointExtensions.cs
public static WebApplication MapCourseEndpoints(this WebApplication app)
{
    app.MapGet("/courses/{idOrSlug}", CourseEndpoints.Get).AllowAnonymous();
    return app;
}
```

### Service Layer

Services encapsulate business logic and coordinate repositories:
- Validation via FluentValidation
- Cross-domain coordination (e.g., OrderService auto-enrolls after purchase)
- Return `null` for "not found" scenarios

### Error Handling

Centralized via `ProblemExceptionHandler` implementing RFC 7807 Problem Details:

```csharp
public class ProblemExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException) return false;
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return await problemDetailsService.TryWriteAsync(/* ... */);
    }
}
```

## Authentication & Authorization

### JWT Bearer Authentication

- Symmetric key validation
- Token claims: `user_id`, `email`, `is_admin`, `jti`
- Auto-admin for `@dometrain.com` email addresses
- Default lifetime: 8 hours

### Authorization Policies

| Policy | Requirement |
|--------|-------------|
| `Admin` | JWT claim `is_admin: true` |
| `ApiAdmin` | JWT admin claim OR `x-api-key` header |

### User Context Extensions

C# 14 extension member syntax (`src/Dometrain.Monolith.Api/Identity/IdentityExtensions.cs`):

```csharp
extension(HttpContext context)
{
    public Guid? GetUserId() { /* extract from claims */ }
    public bool IsAdmin() { /* check is_admin claim */ }
}
```

## Database Schema

### PostgreSQL Tables (managed by EF Core)

```sql
-- students
id UUID PRIMARY KEY, email TEXT UNIQUE, fullname TEXT, password_hash TEXT

-- courses
id UUID PRIMARY KEY, name TEXT, description TEXT, slug TEXT UNIQUE, author TEXT

-- enrollments (composite PK)
PRIMARY KEY (student_id, course_id), FK to students and courses

-- orders
id UUID PRIMARY KEY, student_id UUID FK, created_at_utc TIMESTAMP

-- order_items (composite PK)
PRIMARY KEY (order_id, course_id), FK to orders and courses

-- shopping_carts
id UUID PRIMARY KEY, student_id UUID UNIQUE FK

-- shopping_cart_items (composite PK)
PRIMARY KEY (shopping_cart_id, course_id), FK to shopping_carts and courses
```

### Default Admin User

Created on startup:
- ID: `005d25b1-bfc8-4391-b349-6cec00d1416c`
- Email: `admin@dometrain.com`

## C# 14 / .NET 10 Features Used

| Feature | Example Location |
|---------|-----------------|
| Extension Members | `ServiceDefaults/Extensions.cs`, `Identity/IdentityExtensions.cs` |
| Primary Constructors | `DometrainDbContext.cs`, `CourseRepository.cs`, `CachedCourseRepository.cs` |
| Collection Expressions | Entity navigation properties (`ICollection<OrderItem> OrderItems { get; set; } = [];`) |
| Source-Generated Regex | `Course.cs` (`[GeneratedRegex(...)]`) |
| Required Members | Entity properties (`required Guid Id`) |
| Pattern Matching | Claims checking in `IdentityExtensions.cs` |

## Observability

### OpenTelemetry (via ServiceDefaults)

- **Metrics**: ASP.NET Core, HTTP client, runtime instrumentation
- **Traces**: Application traces, HTTP client, health checks filtered out
- **Logs**: Structured logging with scopes
- **Exporter**: OTLP (configured via `OTEL_EXPORTER_OTLP_ENDPOINT`)

### Prometheus & Grafana

- Prometheus config: `prometheus/prometheus.yml` (scrapes `/metrics`)
- Grafana dashboards: `grafana/config/`

### Health Checks

- `/health` - Readiness (all checks must pass)
- `/alive` - Liveness (only `live` tagged checks)
- Development environment only

## Key Dependencies

### API Project

| Package | Purpose |
|---------|---------|
| `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` | EF Core PostgreSQL integration with Aspire |
| `Microsoft.EntityFrameworkCore.Design` | EF Core tooling (migrations) |
| `Aspire.StackExchange.Redis` | Redis client integration |
| `FluentValidation.DependencyInjectionExtensions` | Request validation |
| `MassTransit.RabbitMQ` | Message broker (infrastructure ready) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT authentication |
| `Swashbuckle.AspNetCore` | OpenAPI/Swagger |

### ServiceDefaults

| Package | Purpose |
|---------|---------|
| `Microsoft.Extensions.Http.Resilience` | HTTP client resilience (retry, circuit breaker) |
| `Microsoft.Extensions.ServiceDiscovery` | Service discovery |
| `OpenTelemetry.*` | Observability instrumentation |

## Dependency Injection

All services registered as **Singleton** (using `IDbContextFactory` for EF Core which creates short-lived DbContext instances per operation):

```csharp
// Infrastructure (EF Core with Aspire)
builder.AddNpgsqlDbContext<DometrainDbContext>("dometrain");
builder.Services.AddDbContextFactory<DometrainDbContext>(lifetime: ServiceLifetime.Singleton);
builder.AddRedisClient("redis");

// Services (singleton)
builder.Services.AddSingleton<IIdentityService, IdentityService>();
builder.Services.AddSingleton<ICourseService, CourseService>();
// ... etc

// Validators (assembly scanning)
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
```

## Important Notes

1. **Always run via Aspire AppHost** - This orchestrates all infrastructure dependencies automatically
2. **EF Core Migrations** - Applied automatically on startup; schema changes require new migrations
3. **RabbitMQ/MassTransit** infrastructure is ready but not yet implemented
4. **Secrets are in appsettings.json** - For production, use Azure Key Vault or environment variables
5. **No cache TTL** - Cache entries persist until explicitly invalidated
6. **Transaction handling** - EF Core manages transactions via `SaveChangesAsync()`

## File Reference Quick Links

| Component | Path |
|-----------|------|
| AppHost entry | `src/Dometrain.Aspire.AppHost/AppHost.cs` |
| ServiceDefaults | `src/Dometrain.Aspire.ServiceDefaults/Extensions.cs` |
| API entry | `src/Dometrain.Monolith.Api/Program.cs` |
| DbContext | `src/Dometrain.Monolith.Api/Database/DometrainDbContext.cs` |
| EF Core Configurations | `src/Dometrain.Monolith.Api/Database/Configurations/` |
| EF Core Migrations | `src/Dometrain.Monolith.Api/Database/Migrations/` |
| Error handler | `src/Dometrain.Monolith.Api/ErrorHandling/ProblemExceptionHandler.cs` |
| Identity service | `src/Dometrain.Monolith.Api/Identity/IIdentityService.cs` |
| Admin auth handler | `src/Dometrain.Monolith.Api/Identity/AdminAuthRequirement.cs` |
| Cached course repo | `src/Dometrain.Monolith.Api/Courses/CachedCourseRepository.cs` |
| Course validator | `src/Dometrain.Monolith.Api/Courses/CourseValidator.cs` |
