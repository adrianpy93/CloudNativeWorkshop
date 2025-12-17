#region

using Dometrain.Monolith.Api.ShoppingCarts.Models;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts.Interfaces;

public interface IShoppingCartService
{
    Task<ShoppingCart?> AddCourseAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> GetByIdAsync(Guid studentId);

    Task<ShoppingCart?> RemoveItemAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> ClearAsync(Guid studentId);
}