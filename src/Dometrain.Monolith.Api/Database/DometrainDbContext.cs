#region

using Dometrain.Monolith.Api.Courses.Models;
using Dometrain.Monolith.Api.Enrollments.Models;
using Dometrain.Monolith.Api.Orders.Models;
using Dometrain.Monolith.Api.ShoppingCarts.Models;
using Dometrain.Monolith.Api.Students.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Dometrain.Monolith.Api.Database;

public class DometrainDbContext(DbContextOptions<DometrainDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DometrainDbContext).Assembly);
    }
}