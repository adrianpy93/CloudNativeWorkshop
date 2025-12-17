#region

using Dometrain.Monolith.Api.Orders.Models;

#endregion

namespace Dometrain.Monolith.Api.Orders.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);
    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}