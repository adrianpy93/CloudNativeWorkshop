using Dometrain.Monolith.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Dometrain.Monolith.Api.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);
    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}

public class OrderRepository(IDbContextFactory<DometrainDbContext> contextFactory) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.Orders
            .Where(o => o.StudentId == studentId)
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Add order items
        foreach (var courseId in courseIds)
        {
            order.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                CourseId = courseId
            });
        }

        context.Orders.Add(order);

        var result = await context.SaveChangesAsync();
        return result > 0 ? order : null;
    }
}
