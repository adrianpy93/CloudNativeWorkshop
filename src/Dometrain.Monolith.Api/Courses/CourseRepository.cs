using Dometrain.Monolith.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Dometrain.Monolith.Api.Courses;

public interface ICourseRepository
{
    Task<Course?> CreateAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetBySlugAsync(string slug);
    Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize);
    Task<Course?> UpdateAsync(Course course);
    Task<bool> DeleteAsync(Guid id);
}

public class CourseRepository(IDbContextFactory<DometrainDbContext> contextFactory) : ICourseRepository
{
    public async Task<Course?> CreateAsync(Course course)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        course.GenerateSlug();
        context.Courses.Add(course);

        var result = await context.SaveChangesAsync();
        return result > 0 ? course : null;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Courses.FindAsync(id);
    }

    public async Task<Course?> GetBySlugAsync(string slug)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Courses.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var query = context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{nameFilter}%"));
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var existingCourse = await context.Courses.FindAsync(course.Id);
        if (existingCourse is null) return null;

        existingCourse.Name = course.Name;
        existingCourse.Description = course.Description;
        existingCourse.Author = course.Author;
        existingCourse.GenerateSlug();

        var result = await context.SaveChangesAsync();
        return result > 0 ? existingCourse : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var course = await context.Courses.FindAsync(id);
        if (course is null) return false;

        context.Courses.Remove(course);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }
}
