#region

using Dometrain.Monolith.Api.Enrollments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Dometrain.Monolith.Api.Database.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        // Composite primary key
        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.StudentId)
            .HasColumnName("student_id");

        builder.Property(e => e.CourseId)
            .HasColumnName("course_id");

        // FK index for Course
        builder.HasIndex(e => e.CourseId);
    }
}