using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // id | uuid | PK
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            //.HasColumnName("id")
            .HasColumnType("uuid");

        // email | varchar(255) | UNIQUE, NOT NULL | Хранить в нижнем регистре
        builder.Property(u => u.Email)
            //.HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired()
            .HasComment("Храним в нижнем регистре");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // password_hash | varchar(255) | NOT NULL | Хеш, не пароль
        builder.Property(u => u.PasswordHash)
            //.HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired()
            .HasComment("Хеш, не пароль");

        // display_name | varchar(100) | NOT NULL | Имя для шапки
        builder.Property(u => u.DisplayName)
            //.HasColumnName("display_name")
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("Имя для шапки");

        // created_at | timestamptz | NOT NULL
        builder.Property(u => u.CreatedAt)
            //.HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}