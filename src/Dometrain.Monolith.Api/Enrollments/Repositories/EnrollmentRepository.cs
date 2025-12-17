#region

using Dometrain.Monolith.Api.Database;
using Dometrain.Monolith.Api.Enrollments.Interfaces;
using Dometrain.Monolith.Api.Enrollments.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.Enrollments.Repositories;

public class EnrollmentRepository(IDbContextFactory<DometrainDbContext> contextFactory) : IEnrollmentRepository
{
    public async Task<IEnumerable<Guid>> GetEnrolledCoursesAsync(Guid studentId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync();
    }

    public async Task<bool> EnrollToCourseAsync(Guid studentId, Guid courseId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        // Check if already enrolled (equivalent to ON CONFLICT DO NOTHING)
        var exists = await context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (exists) return false;

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId
        };

        context.Enrollments.Add(enrollment);

        try
        {
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
        catch (DbUpdateException)
        {
            // Handle race condition where enrollment was added between check and insert
            return false;
        }
    }

    public async Task<bool> UnEnrollFromCourseAsync(Guid studentId, Guid courseId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var enrollment = await context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (enrollment is null) return false;

        context.Enrollments.Remove(enrollment);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }
}