using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        // id | uuid | PK
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            //.HasColumnName("id")
            .HasColumnType("uuid");

        // user_id | uuid | FK -> users, NOT NULL
        builder.Property(rt => rt.UserId)
            //.HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        // Связь FK -> users (при удалении пользователя удаляются и его токены)
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // token_hash | varchar(255) | UNIQUE, NOT NULL | Хеш токена (SHA-256), сам токен не храним
        builder.Property(rt => rt.TokenHash)
            //.HasColumnName("token_hash")
            .HasMaxLength(255)
            .IsRequired()
            .HasComment("Хеш токена (SHA-256), сам токен не храним");

        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

        // expires_at | timestamptz | NOT NULL
        builder.Property(rt => rt.ExpiresAt)
            //.HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        // revoked_at | timestamptz | NULL | Заполнено, если токен отозван
        builder.Property(rt => rt.RevokedAt)
            //.HasColumnName("revoked_at")
            .HasColumnType("timestamptz")
            .IsRequired(false)
            .HasComment("Заполнено, если токен отозван");

        // created_at | timestamptz | NOT NULL
        builder.Property(rt => rt.CreatedAt)
            //.HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}
