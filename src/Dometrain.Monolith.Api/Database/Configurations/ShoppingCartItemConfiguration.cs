#region

using Dometrain.Monolith.Api.ShoppingCarts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Dometrain.Monolith.Api.Database.Configurations;

public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
{
    public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.ToTable("shopping_cart_items");

        // Composite primary key
        builder.HasKey(i => new { i.ShoppingCartId, i.CourseId });

        builder.Property(i => i.ShoppingCartId)
            .HasColumnName("shopping_cart_id");

        builder.Property(i => i.CourseId)
            .HasColumnName("course_id");

        // FK index for Course
        builder.HasIndex(i => i.CourseId);
    }
}