#region

using Dometrain.Monolith.Api.Courses.Models;
using Dometrain.Monolith.Api.Enrollments.Models;

#endregion

namespace Dometrain.Monolith.Api.Database;

public sealed record SeedData(
    IReadOnlyList<SeedStudent> Students,
    IReadOnlyList<Course> Courses,
    IReadOnlyList<SeedOrder> Orders,
    IReadOnlyList<Enrollment> Enrollments
);

// Student needs Password field for seeding (domain model has PasswordHash)
public sealed record SeedStudent(
    Guid Id,
    string Email,
    string FullName,
    string Password
);

// Order needs CreatedDaysAgo and direct CourseIds (domain model has DateTime and OrderItems navigation)
public sealed record SeedOrder(
    Guid Id,
    Guid StudentId,
    int CreatedDaysAgo,
    IReadOnlyList<Guid> CourseIds
);