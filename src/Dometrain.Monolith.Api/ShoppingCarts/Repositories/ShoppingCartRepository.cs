#region

using Dometrain.Monolith.Api.Database;
using Dometrain.Monolith.Api.ShoppingCarts.Interfaces;
using Dometrain.Monolith.Api.ShoppingCarts.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.ShoppingCarts.Repositories;

public class ShoppingCartRepository(IDbContextFactory<DometrainDbContext> contextFactory) : IShoppingCartRepository
{
    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        // Get or create cart for student
        var cart = await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == studentId);

        if (cart is null)
        {
            cart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                StudentId = studentId
            };
            context.ShoppingCarts.Add(cart);
        }

        // Check if course already in cart
        if (cart.Items.Any(i => i.CourseId == courseId)) return true; // Already exists, consider it a success

        // Add course to cart
        cart.Items.Add(new ShoppingCartItem
        {
            ShoppingCartId = cart.Id,
            CourseId = courseId
        });

        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == studentId);
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var cart = await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == studentId);

        if (cart is null) return true; // No cart means item isn't there

        var item = cart.Items.FirstOrDefault(i => i.CourseId == courseId);
        if (item is null) return true; // Item not in cart

        cart.Items.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var cart = await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == studentId);

        if (cart is null) return true; // No cart to clear

        // Remove the cart (cascade will delete items)
        context.ShoppingCarts.Remove(cart);
        await context.SaveChangesAsync();
        return true;
    }
}