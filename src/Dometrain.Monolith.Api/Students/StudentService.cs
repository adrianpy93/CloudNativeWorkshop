#region

using FluentValidation;
using Microsoft.AspNetCore.Identity;

#endregion

namespace Dometrain.Monolith.Api.Students;

public interface IStudentService
{
    Task<bool> CheckCredentialsAsync(string email, string password);

    Task<Student?> CreateAsync(Student student, string password);

    Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize);

    Task<Student?> GetByEmailAsync(string email);

    Task<Student?> GetByIdAsync(Guid id);

    Task<bool> DeleteAsync(Guid id);
}

public class StudentService(
    IPasswordHasher<Student> passwordHasher,
    IStudentRepository studentRepository,
    IValidator<Student> validator
) : IStudentService
{
    public async Task<bool> CheckCredentialsAsync(string email, string password)
    {
        var storedHash = await studentRepository.GetPasswordHashAsync(email);

        if (storedHash is null) return false;

        var valid = passwordHasher.VerifyHashedPassword(null!, storedHash, password);
        return valid == PasswordVerificationResult.Success;
    }

    public async Task<Student?> CreateAsync(Student student, string password)
    {
        await validator.ValidateAndThrowAsync(student);
        var hash = passwordHasher.HashPassword(null!, password);
        return await studentRepository.CreateAsync(student, hash);
    }

    public async Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;

        return await studentRepository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await studentRepository.GetByEmailAsync(email);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await studentRepository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await studentRepository.DeleteByIdAsync(id);
    }
}