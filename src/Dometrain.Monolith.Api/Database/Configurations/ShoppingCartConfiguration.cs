#region

using Dometrain.Monolith.Api.ShoppingCarts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Dometrain.Monolith.Api.Database.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("shopping_carts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        // Unique constraint on StudentId (one cart per student)
        builder.HasIndex(c => c.StudentId)
            .IsUnique()
            .HasDatabaseName("shopping_carts_student_id_index");

        // Ignore the computed CourseIds property
        builder.Ignore(c => c.CourseIds);

        // Navigation to ShoppingCartItems (one-way)
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}