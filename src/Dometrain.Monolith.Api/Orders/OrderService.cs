#region

using Dometrain.Monolith.Api.Enrollments;
using Dometrain.Monolith.Api.Students;

#endregion

namespace Dometrain.Monolith.Api.Orders;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(Guid orderId);

    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);

    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}

public class OrderService(
    IOrderRepository orderRepository,
    IStudentRepository studentRepository,
    IEnrollmentService enrollmentService
) : IOrderService
{
    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await orderRepository.GetByIdAsync(orderId);
    }

    public async Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId)
    {
        return await orderRepository.GetAllForStudentAsync(studentId);
    }

    public async Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student is null) return null;

        var order = await orderRepository.PlaceAsync(studentId, courseIds);

        if (order is null) return null;

        var enrollments = await enrollmentService.GetStudentEnrollmentsAsync(studentId);

        foreach (var courseId in order.CourseIds.Where(x => !enrollments!.CourseIds.Contains(x)))
            await enrollmentService.EnrollToCourseAsync(studentId, courseId);

        return order;
    }
}