#region

using Dometrain.Monolith.Api.Database;
using Dometrain.Monolith.Api.Students.Interfaces;
using Dometrain.Monolith.Api.Students.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.Students.Repositories;

public class StudentRepository(IDbContextFactory<DometrainDbContext> contextFactory) : IStudentRepository
{
    public async Task<string?> GetPasswordHashAsync(string email)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var student = await context.Students.FirstOrDefaultAsync(s => s.Email == email);
        return student?.PasswordHash;
    }

    public async Task<Student?> CreateAsync(Student student, string hash)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        student.PasswordHash = hash;
        context.Students.Add(student);

        var result = await context.SaveChangesAsync();
        return result > 0 ? student : null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Students.AnyAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Students
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Students.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Students.FindAsync(id);
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var student = await context.Students.FindAsync(id);
        if (student is null) return false;

        context.Students.Remove(student);
        var result = await context.SaveChangesAsync();
        return result > 0;
    }
}