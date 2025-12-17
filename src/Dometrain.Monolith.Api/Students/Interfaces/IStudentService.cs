#region

using Dometrain.Monolith.Api.Students.Models;

#endregion

namespace Dometrain.Monolith.Api.Students.Interfaces;

public interface IStudentService
{
    Task<bool> CheckCredentialsAsync(string email, string password);

    Task<Student?> CreateAsync(Student student, string password);

    Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize);

    Task<Student?> GetByEmailAsync(string email);

    Task<Student?> GetByIdAsync(Guid id);

    Task<bool> DeleteAsync(Guid id);
}