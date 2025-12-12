#region

using Dometrain.Monolith.Api.Courses;
using Dometrain.Monolith.Api.Students;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts;

public interface IShoppingCartService
{
    Task<ShoppingCart?> AddCourseAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> GetByIdAsync(Guid studentId);

    Task<ShoppingCart?> RemoveItemAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> ClearAsync(Guid studentId);
}

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