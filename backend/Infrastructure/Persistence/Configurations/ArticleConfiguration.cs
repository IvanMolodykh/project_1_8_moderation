using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");


        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnType("uuid");


        builder.Property(a => a.AuthorId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Content)
            .HasColumnType("text")
            .IsRequired();


        builder.Property(a => a.ExtraData)
            .HasColumnType("jsonb")
            .IsRequired(false);

  
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();


        builder.Property(a => a.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();


        builder.Property(a => a.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(a => a.AuthorId);
    }
}