using Dometrain.Monolith.Api.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dometrain.Monolith.Api.Database.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.Email)
            .HasColumnName("email")
            .HasMaxLength(254) // RFC 5321 max email length
            .IsRequired();

        builder.Property(s => s.FullName)
            .HasColumnName("fullname")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(512) // Supports bcrypt, argon2, PBKDF2 hashes
            .IsRequired();

        builder.HasIndex(s => s.Email)
            .IsUnique()
            .HasDatabaseName("students_email_index");
    }
}
