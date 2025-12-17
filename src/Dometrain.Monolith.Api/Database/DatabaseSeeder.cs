#region

using Dometrain.Monolith.Api.Courses.Models;
using Dometrain.Monolith.Api.Enrollments.Models;
using Dometrain.Monolith.Api.Orders.Models;
using Dometrain.Monolith.Api.Students.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.Database;

public static class DatabaseSeeder
{
    // Fixed IDs for reproducible seed data
    private static readonly Guid AdminId = Guid.Parse("005d25b1-bfc8-4391-b349-6cec00d1416c");
    private static readonly Guid Student1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Student2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Student3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid Course1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid Course2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid Course3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid Course4Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid Course5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    private static readonly Guid Order1Id = Guid.Parse("11110000-0000-0000-0000-000000000001");
    private static readonly Guid Order2Id = Guid.Parse("22220000-0000-0000-0000-000000000002");

    public static async Task SeedAsync(DometrainDbContext context, IPasswordHasher<Student> passwordHasher)
    {
        await SeedStudentsAsync(context, passwordHasher);
        await SeedCoursesAsync(context);
        await context.SaveChangesAsync();

        await SeedOrdersAsync(context);
        await SeedEnrollmentsAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedStudentsAsync(DometrainDbContext context, IPasswordHasher<Student> passwordHasher)
    {
        // Admin user
        if (!await context.Students.AnyAsync(s => s.Id == AdminId))
        {
            var admin = new Student { Id = AdminId, Email = "admin@dometrain.com", FullName = "Admin" };
            admin.PasswordHash = passwordHasher.HashPassword(admin, "admin");
            context.Students.Add(admin);
        }

        // Sample students
        if (!await context.Students.AnyAsync(s => s.Id == Student1Id))
        {
            var students = new[]
            {
                new Student { Id = Student1Id, Email = "john.doe@example.com", FullName = "John Doe" },
                new Student { Id = Student2Id, Email = "jane.smith@example.com", FullName = "Jane Smith" },
                new Student { Id = Student3Id, Email = "bob.wilson@example.com", FullName = "Bob Wilson" }
            };

            foreach (var student in students)
                student.PasswordHash = passwordHasher.HashPassword(student, "password123");

            context.Students.AddRange(students);
        }
    }

    private static async Task SeedCoursesAsync(DometrainDbContext context)
    {
        if (await context.Courses.AnyAsync(c => c.Id == Course1Id))
            return;

        var courses = new[]
        {
            new Course
            {
                Id = Course1Id,
                Name = "Getting Started with .NET Aspire",
                Description = "Learn how to build cloud-native applications with .NET Aspire orchestration, service defaults, and local development experience.",
                Author = "Nick Chapsas",
                Slug = "getting-started-with-dotnet-aspire"
            },
            new Course
            {
                Id = Course2Id,
                Name = "Deep Dive into Entity Framework Core",
                Description = "Master Entity Framework Core from basics to advanced topics including performance optimization, migrations, and complex queries.",
                Author = "Julie Lerman",
                Slug = "deep-dive-into-entity-framework-core"
            },
            new Course
            {
                Id = Course3Id,
                Name = "Mastering Minimal APIs",
                Description = "Build high-performance APIs with ASP.NET Core Minimal APIs, learn endpoint routing, filters, and best practices.",
                Author = "Nick Chapsas",
                Slug = "mastering-minimal-apis"
            },
            new Course
            {
                Id = Course4Id,
                Name = "Redis Caching Strategies",
                Description = "Implement effective caching strategies with Redis, including distributed caching, cache invalidation, and performance patterns.",
                Author = "Steve Gordon",
                Slug = "redis-caching-strategies"
            },
            new Course
            {
                Id = Course5Id,
                Name = "RabbitMQ and MassTransit Fundamentals",
                Description = "Learn message-driven architecture with RabbitMQ and MassTransit, including pub/sub, sagas, and fault tolerance.",
                Author = "Chris Patterson",
                Slug = "rabbitmq-and-masstransit-fundamentals"
            }
        };

        context.Courses.AddRange(courses);
    }

    private static async Task SeedOrdersAsync(DometrainDbContext context)
    {
        if (await context.Orders.AnyAsync(o => o.Id == Order1Id))
            return;

        var orders = new[]
        {
            new Order { Id = Order1Id, StudentId = Student1Id, CreatedAtUtc = DateTime.UtcNow.AddDays(-30) },
            new Order { Id = Order2Id, StudentId = Student2Id, CreatedAtUtc = DateTime.UtcNow.AddDays(-15) }
        };
        context.Orders.AddRange(orders);

        var orderItems = new[]
        {
            // John Doe purchased Aspire and EF Core courses
            new OrderItem { OrderId = Order1Id, CourseId = Course1Id },
            new OrderItem { OrderId = Order1Id, CourseId = Course2Id },
            // Jane Smith purchased Minimal APIs, Redis, and RabbitMQ courses
            new OrderItem { OrderId = Order2Id, CourseId = Course3Id },
            new OrderItem { OrderId = Order2Id, CourseId = Course4Id },
            new OrderItem { OrderId = Order2Id, CourseId = Course5Id }
        };
        context.OrderItems.AddRange(orderItems);
    }

    private static async Task SeedEnrollmentsAsync(DometrainDbContext context)
    {
        if (await context.Enrollments.AnyAsync(e => e.StudentId == Student1Id && e.CourseId == Course1Id))
            return;

        var enrollments = new[]
        {
            // John Doe enrolled in courses from his order
            new Enrollment { StudentId = Student1Id, CourseId = Course1Id },
            new Enrollment { StudentId = Student1Id, CourseId = Course2Id },
            // Jane Smith enrolled in courses from her order
            new Enrollment { StudentId = Student2Id, CourseId = Course3Id },
            new Enrollment { StudentId = Student2Id, CourseId = Course4Id },
            new Enrollment { StudentId = Student2Id, CourseId = Course5Id },
            // Bob Wilson enrolled in some courses
            new Enrollment { StudentId = Student3Id, CourseId = Course1Id },
            new Enrollment { StudentId = Student3Id, CourseId = Course3Id }
        };

        context.Enrollments.AddRange(enrollments);
    }
}
