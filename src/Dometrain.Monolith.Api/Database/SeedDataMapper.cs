#region

using Dometrain.Monolith.Api.Orders.Models;
using Dometrain.Monolith.Api.Students.Models;
using Microsoft.AspNetCore.Identity;

#endregion

namespace Dometrain.Monolith.Api.Database;

public static class SeedDataMapper
{
    public static Student ToStudent(SeedStudent seed, IPasswordHasher<Student> passwordHasher)
    {
        var student = new Student
        {
            Id = seed.Id,
            Email = seed.Email,
            FullName = seed.FullName
        };
        student.PasswordHash = passwordHasher.HashPassword(student, seed.Password);
        return student;
    }

    public static (Order Order, IEnumerable<OrderItem> Items) ToOrder(SeedOrder seed)
    {
        var order = new Order
        {
            Id = seed.Id,
            StudentId = seed.StudentId,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-seed.CreatedDaysAgo)
        };

        var items = seed.CourseIds.Select(courseId => new OrderItem
        {
            OrderId = seed.Id,
            CourseId = courseId
        });

        return (order, items);
    }
}