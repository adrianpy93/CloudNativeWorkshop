#region

using System.Text.Json.Serialization;

#endregion

namespace Dometrain.Monolith.Api.Orders.Models;

public class Order
{
    public required Guid Id { get; init; }

    public required Guid StudentId { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    // Navigation property for EF Core Include
    [JsonIgnore] public ICollection<OrderItem> OrderItems { get; set; } = [];

    // Computed property for backward compatibility with existing API
    public IEnumerable<Guid> CourseIds => OrderItems.Select(oi => oi.CourseId);
}