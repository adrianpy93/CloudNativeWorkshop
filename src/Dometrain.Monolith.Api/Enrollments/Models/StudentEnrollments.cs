namespace Dometrain.Monolith.Api.Enrollments.Models;

public class StudentEnrollments
{
    public Guid StudentId { get; init; }
    public List<Guid> CourseIds { get; init; } = [];
}