namespace Dometrain.Monolith.Api.Orders.Models;

public class OrderItem
{
    public Guid OrderId { get; set; }

    public Guid CourseId { get; set; }
}