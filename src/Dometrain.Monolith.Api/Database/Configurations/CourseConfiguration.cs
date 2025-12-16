using Dometrain.Monolith.Api.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dometrain.Monolith.Api.Database.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200) // URL-friendly identifier
            .IsRequired();

        builder.Property(c => c.Author)
            .HasColumnName("author")
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("courses_slug_index");
    }
}
