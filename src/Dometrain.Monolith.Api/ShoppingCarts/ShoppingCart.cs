using System.Text.Json.Serialization;

namespace Dometrain.Monolith.Api.ShoppingCarts;

public class ShoppingCart
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    // Navigation property for EF Core Include
    [JsonIgnore]
    public ICollection<ShoppingCartItem> Items { get; set; } = [];

    // Computed property for backward compatibility with existing API
    public IEnumerable<Guid> CourseIds => Items.Select(i => i.CourseId);
}
