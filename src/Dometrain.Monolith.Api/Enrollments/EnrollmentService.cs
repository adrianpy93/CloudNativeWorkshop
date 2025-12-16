#region

using Dometrain.Monolith.Api.Courses;
using Dometrain.Monolith.Api.Students;

#endregion

namespace Dometrain.Monolith.Api.Enrollments;

public interface IEnrollmentService
{
    Task<StudentEnrollments?> GetStudentEnrollmentsAsync(Guid studentId);

    Task<bool?> EnrollToCourseAsync(Guid studentId, Guid courseId);

    Task<bool?> UnEnrollFromCourseAsync(Guid studentId, Guid courseId);
}

public class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ICourseRepository courseRepository)
    : IEnrollmentService
{
    public async Task<StudentEnrollments?> GetStudentEnrollmentsAsync(Guid studentId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);

        if (student is null) return null;

        var courseIds = await enrollmentRepository.GetEnrolledCoursesAsync(studentId);
        return new StudentEnrollments
        {
            StudentId = studentId, CourseIds = courseIds.ToList()
        };
    }

    public async Task<bool?> EnrollToCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);

        if (student is null) return null;

        var course = await courseRepository.GetByIdAsync(courseId);

        if (course is null) return null;

        return await enrollmentRepository.EnrollToCourseAsync(studentId, courseId);
    }

    public async Task<bool?> UnEnrollFromCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);

        if (student is null) return null;

        var course = await courseRepository.GetByIdAsync(courseId);

        if (course is null) return null;

        return await enrollmentRepository.UnEnrollFromCourseAsync(studentId, courseId);
    }
}