#region

using System.Text.Json;
using Dometrain.Monolith.Api.ShoppingCarts.Interfaces;
using Dometrain.Monolith.Api.ShoppingCarts.Models;
using StackExchange.Redis;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts.Repositories;

public class CachedShoppingCartRepository(
    IShoppingCartRepository shoppingCartRepository,
    IConnectionMultiplexer connectionMultiplexer
) : IShoppingCartRepository
{
    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        var added = await shoppingCartRepository.AddCourseAsync(studentId, courseId);

        if (!added) return added;

        var db = connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync($"cart:id:{studentId}");
        return added;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        var db = connectionMultiplexer.GetDatabase();
        var cachedCartString = await db.StringGetAsync($"cart:id:{studentId}");
        if (!cachedCartString.IsNull) return JsonSerializer.Deserialize<ShoppingCart>(cachedCartString.ToString());

        var cart = await shoppingCartRepository.GetByIdAsync(studentId);
        await db.StringSetAsync($"cart:id:{studentId}", JsonSerializer.Serialize(cart));
        return cart;
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        var removed = await shoppingCartRepository.RemoveItemAsync(studentId, courseId);
        if (!removed) return removed;

        var db = connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync($"cart:id:{studentId}");
        return removed;
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        var cleared = await shoppingCartRepository.ClearAsync(studentId);
        if (!cleared) return cleared;

        var db = connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync($"cart:id:{studentId}");
        return cleared;
    }
}