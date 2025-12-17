namespace Dometrain.Monolith.Api.Enrollments.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Guid>> GetEnrolledCoursesAsync(Guid studentId);
    Task<bool> EnrollToCourseAsync(Guid studentId, Guid courseId);
    Task<bool> UnEnrollFromCourseAsync(Guid studentId, Guid courseId);
}