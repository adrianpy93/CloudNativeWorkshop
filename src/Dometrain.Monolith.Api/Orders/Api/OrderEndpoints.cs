#region

using Dometrain.Monolith.Api.Identity;
using Dometrain.Monolith.Api.Orders.Interfaces;
using Dometrain.Monolith.Api.Orders.Requests;

#endregion

namespace Dometrain.Monolith.Api.Orders.Api;

public static class OrderEndpoints
{
    public const string GetOrderEndpointName = "GetOrderById";

    public static async Task<IResult> Place(PlaceOrderRequest request, IOrderService orderService,
        HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var createdOrder = await orderService.PlaceAsync(studentId.Value, request.CourseIds);

        return createdOrder is null
            ? Results.NotFound()
            : TypedResults.CreatedAtRoute(createdOrder, GetOrderEndpointName, new { orderId = createdOrder.Id });
    }

    public static async Task<IResult> Get(Guid orderId, IOrderService orderService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var order = await orderService.GetByIdAsync(orderId);

        if (order is null || (order.StudentId != studentId && !httpContext.IsAdmin()))
            return Results.NotFound();

        return Results.Ok(order);
    }

    public static async Task<IResult> GetAllForStudent(IOrderService orderService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var orders = await orderService.GetAllForStudentAsync(studentId.Value);
        return Results.Ok(orders);
    }
}