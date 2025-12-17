#region

using Dometrain.Monolith.Api.Courses.Models;

#endregion

namespace Dometrain.Monolith.Api.Courses.Interfaces;

public interface ICourseRepository
{
    Task<Course?> CreateAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetBySlugAsync(string slug);
    Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize);
    Task<Course?> UpdateAsync(Course course);
    Task<bool> DeleteAsync(Guid id);
}