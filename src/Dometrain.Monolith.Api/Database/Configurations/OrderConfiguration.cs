#region

using Dometrain.Monolith.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Dometrain.Monolith.Api.Database.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        builder.Property(o => o.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        // Ignore the computed CourseIds property
        builder.Ignore(o => o.CourseIds);

        // FK to Student
        builder.HasIndex(o => o.StudentId);

        // Navigation to OrderItems (one-way)
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}