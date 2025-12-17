namespace Dometrain.Monolith.Api.Orders.Requests;

public class PlaceOrderRequest
{
    public required IEnumerable<Guid> CourseIds { get; init; }
}