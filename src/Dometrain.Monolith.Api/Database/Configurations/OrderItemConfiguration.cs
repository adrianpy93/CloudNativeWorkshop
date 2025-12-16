using Dometrain.Monolith.Api.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dometrain.Monolith.Api.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        // Composite primary key
        builder.HasKey(oi => new { oi.OrderId, oi.CourseId });

        builder.Property(oi => oi.OrderId)
            .HasColumnName("order_id");

        builder.Property(oi => oi.CourseId)
            .HasColumnName("course_id");

        // FK index for Course
        builder.HasIndex(oi => oi.CourseId);
    }
}
