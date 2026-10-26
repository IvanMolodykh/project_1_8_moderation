using Domain.Enums;

namespace Domain.Entities;

public class Article
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;


    public string? ExtraData { get; set; }

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    public User Author { get; set; } = null!;
}