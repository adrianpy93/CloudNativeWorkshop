#region

using System.Text.Json;
using Dometrain.Monolith.Api.Courses.Models;
using Dometrain.Monolith.Api.Enrollments.Models;
using Dometrain.Monolith.Api.Students.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.Database;

public static class DatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedAsync(DometrainDbContext context, IPasswordHasher<Student> passwordHasher,
        string seedFilePath)
    {
        var seedData = await LoadSeedDataAsync(seedFilePath);

        await SeedStudentsAsync(context, passwordHasher, seedData.Students);
        await SeedCoursesAsync(context, seedData.Courses);
        await SeedOrdersAsync(context, seedData.Orders);
        await SeedEnrollmentsAsync(context, seedData.Enrollments);

        await context.SaveChangesAsync();
    }

    private static async Task<SeedData> LoadSeedDataAsync(string seedFilePath)
    {
        if (!File.Exists(seedFilePath))
            throw new FileNotFoundException($"Seed data file not found: {seedFilePath}");

        await using var stream = File.OpenRead(seedFilePath);
        var seedData = await JsonSerializer.DeserializeAsync<SeedData>(stream, JsonOptions);

        return seedData ?? throw new InvalidOperationException("Failed to deserialize seed data");
    }

    private static async Task SeedStudentsAsync(
        DometrainDbContext context,
        IPasswordHasher<Student> passwordHasher,
        IReadOnlyList<SeedStudent> seedStudents)
    {
        foreach (var seed in seedStudents)
        {
            if (await context.Students.AnyAsync(s => s.Id == seed.Id))
                continue;

            context.Students.Add(SeedDataMapper.ToStudent(seed, passwordHasher));
        }
    }

    private static async Task SeedCoursesAsync(DometrainDbContext context, IReadOnlyList<Course> courses)
    {
        foreach (var course in courses)
        {
            if (await context.Courses.AnyAsync(c => c.Id == course.Id))
                continue;

            context.Courses.Add(course);
        }
    }

    private static async Task SeedOrdersAsync(DometrainDbContext context, IReadOnlyList<SeedOrder> seedOrders)
    {
        foreach (var seed in seedOrders)
        {
            if (await context.Orders.AnyAsync(o => o.Id == seed.Id))
                continue;

            var (order, items) = SeedDataMapper.ToOrder(seed);
            context.Orders.Add(order);
            context.OrderItems.AddRange(items);
        }
    }

    private static async Task SeedEnrollmentsAsync(DometrainDbContext context, IReadOnlyList<Enrollment> enrollments)
    {
        foreach (var enrollment in enrollments)
        {
            if (await context.Enrollments.AnyAsync(e =>
                    e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId))
                continue;

            context.Enrollments.Add(enrollment);
        }
    }
}