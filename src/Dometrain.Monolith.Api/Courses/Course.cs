using System.Text.RegularExpressions;

namespace Dometrain.Monolith.Api.Courses;

public partial class Course
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public string Slug { get; set; } = string.Empty;

    public required string Author { get; set; }

    public void GenerateSlug()
    {
        Slug = SlugRegex().Replace(Name, string.Empty)
            .ToLower().Replace(" ", "-");
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 500)]
    private static partial Regex SlugRegex();
}
