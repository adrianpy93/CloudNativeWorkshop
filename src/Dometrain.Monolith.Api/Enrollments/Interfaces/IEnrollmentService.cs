#region

using Dometrain.Monolith.Api.Enrollments.Models;

#endregion

namespace Dometrain.Monolith.Api.Enrollments.Interfaces;

public interface IEnrollmentService
{
    Task<StudentEnrollments?> GetStudentEnrollmentsAsync(Guid studentId);

    Task<bool?> EnrollToCourseAsync(Guid studentId, Guid courseId);

    Task<bool?> UnEnrollFromCourseAsync(Guid studentId, Guid courseId);
}