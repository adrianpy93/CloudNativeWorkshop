namespace Dometrain.Monolith.Api.Enrollments;

public class StudentEnrollments
{
    public Guid StudentId { get; init; }
    public List<Guid> CourseIds { get; init; } = [];
}
