#region

using Dometrain.Monolith.Api.Students.Models;

#endregion

namespace Dometrain.Monolith.Api.Students.Interfaces;

public interface IStudentRepository
{
    Task<string?> GetPasswordHashAsync(string email);
    Task<Student?> CreateAsync(Student student, string hash);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize);
    Task<Student?> GetByEmailAsync(string email);
    Task<Student?> GetByIdAsync(Guid id);
    Task<bool> DeleteByIdAsync(Guid id);
}