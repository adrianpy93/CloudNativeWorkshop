#region

using System.Text;
using Dometrain.Aspire.ServiceDefaults;
using Dometrain.Monolith.Api.Courses.Api;
using Dometrain.Monolith.Api.Courses.Interfaces;
using Dometrain.Monolith.Api.Courses.Repositories;
using Dometrain.Monolith.Api.Courses.Services;
using Dometrain.Monolith.Api.Database;
using Dometrain.Monolith.Api.Enrollments.Api;
using Dometrain.Monolith.Api.Enrollments.Interfaces;
using Dometrain.Monolith.Api.Enrollments.Repositories;
using Dometrain.Monolith.Api.Enrollments.Services;
using Dometrain.Monolith.Api.ErrorHandling;
using Dometrain.Monolith.Api.Identity;
using Dometrain.Monolith.Api.Identity.Api;
using Dometrain.Monolith.Api.Identity.Interfaces;
using Dometrain.Monolith.Api.Identity.Services;
using Dometrain.Monolith.Api.OpenApi;
using Dometrain.Monolith.Api.Orders.Api;
using Dometrain.Monolith.Api.Orders.Interfaces;
using Dometrain.Monolith.Api.Orders.Repositories;
using Dometrain.Monolith.Api.Orders.Services;
using Dometrain.Monolith.Api.ShoppingCarts.Api;
using Dometrain.Monolith.Api.ShoppingCarts.Interfaces;
using Dometrain.Monolith.Api.ShoppingCarts.Repositories;
using Dometrain.Monolith.Api.ShoppingCarts.Services;
using Dometrain.Monolith.Api.Students.Api;
using Dometrain.Monolith.Api.Students.Interfaces;
using Dometrain.Monolith.Api.Students.Models;
using Dometrain.Monolith.Api.Students.Repositories;
using Dometrain.Monolith.Api.Students.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var config = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Identity:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Identity:Issuer"],
        ValidAudience = config["Identity:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApiAdmin", p => p.AddRequirements(new AdminAuthRequirement(config["Identity:AdminApiKey"]!)))
    .AddPolicy("Admin", p => p.RequireAssertion(c =>
        c.User.HasClaim(m => m is { Type: "is_admin", Value: "true" })));

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(IdentitySettings.SettingsKey));

builder.AddNpgsqlDbContext<DometrainDbContext>("dometrain",
    settings =>
    {
        // Disable SSL for local development - Aspire PostgreSQL container doesn't have SSL configured
        if (!string.IsNullOrEmpty(settings.ConnectionString) &&
            !settings.ConnectionString.Contains("SSL Mode", StringComparison.OrdinalIgnoreCase))
            settings.ConnectionString += ";SSL Mode=Disable";
    });
builder.Services.AddDbContextFactory<DometrainDbContext>(lifetime: ServiceLifetime.Singleton);
builder.AddRedisClient("redis");

builder.Services.AddSingleton<IPasswordHasher<Student>, PasswordHasher<Student>>();
builder.Services.AddSingleton<IIdentityService, IdentityService>();

builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();

builder.Services.AddSingleton<ICourseService, CourseService>();

builder.Services.AddSingleton<CourseRepository>();
builder.Services.AddSingleton<ICourseRepository>(s =>
    new CachedCourseRepository(s.GetRequiredService<CourseRepository>(),
        s.GetRequiredService<IConnectionMultiplexer>()));

builder.Services.AddSingleton<ShoppingCartRepository>();
builder.Services.AddSingleton<IShoppingCartRepository>(s =>
    new CachedShoppingCartRepository(s.GetRequiredService<ShoppingCartRepository>(),
        s.GetRequiredService<IConnectionMultiplexer>()));
builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();

builder.Services.AddSingleton<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();

builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityEndpoints();
app.MapStudentEndpoints();
app.MapCourseEndpoints();
app.MapShoppingCartEndpoints();
app.MapEnrollmentEndpoints();
app.MapOrderEndpoints();

// Apply EF Core migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    await using (var context = scope.ServiceProvider.GetRequiredService<DometrainDbContext>())
    {
        await context.Database.MigrateAsync();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Student>>();
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Database", "seed-data.json");
        await DatabaseSeeder.SeedAsync(context, passwordHasher, seedFilePath);
    }
}

await app.RunAsync();