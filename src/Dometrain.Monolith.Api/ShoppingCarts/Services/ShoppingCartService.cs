#region

using Dometrain.Monolith.Api.Courses.Interfaces;
using Dometrain.Monolith.Api.ShoppingCarts.Interfaces;
using Dometrain.Monolith.Api.ShoppingCarts.Models;
using Dometrain.Monolith.Api.Students.Interfaces;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts.Services;

public class ShoppingCartService(
    IShoppingCartRepository shoppingCartRepository,
    IStudentRepository studentRepository,
    ICourseRepository courseRepository
) : IShoppingCartService
{
    public async Task<ShoppingCart?> AddCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student is null) return null;

        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null) return null;

        await shoppingCartRepository.AddCourseAsync(studentId, courseId);
        return await GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        return await shoppingCartRepository.GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student is null) return null;

        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null) return null;

        await shoppingCartRepository.RemoveItemAsync(studentId, courseId);
        return await GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> ClearAsync(Guid studentId)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student is null) return null;

        await shoppingCartRepository.ClearAsync(studentId);
        return new ShoppingCart
        {
            StudentId = studentId
        };
    }
}