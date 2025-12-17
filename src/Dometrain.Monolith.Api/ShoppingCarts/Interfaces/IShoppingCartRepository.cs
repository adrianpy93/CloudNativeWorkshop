#region

using Dometrain.Monolith.Api.ShoppingCarts.Models;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts.Interfaces;

public interface IShoppingCartRepository
{
    Task<bool> AddCourseAsync(Guid studentId, Guid courseId);
    Task<ShoppingCart?> GetByIdAsync(Guid studentId);
    Task<bool> RemoveItemAsync(Guid studentId, Guid courseId);
    Task<bool> ClearAsync(Guid studentId);
}