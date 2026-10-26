using System.Text.Json;
using Application.DTOs.articles;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ArticleService(IAppDbContext db) : IArticleService
{
    public async Task<List<GetArticleDto>> GetMyArticlesAsync(Guid authorId, CancellationToken ct = default)
    {
        var articles = await db.Articles
            .AsNoTracking()
            .Where(a => a.AuthorId == authorId)
            .OrderByDescending(a => a.UpdatedAt)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Status,
                a.UpdatedAt
            })
            .ToListAsync(ct);

        return articles
            .Select(a => new GetArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Status = a.Status.ToString(),
                UpdatedAt = a.UpdatedAt
            })
            .ToList();
    }

    public async Task<GetArticleDto> CreateArticleAsync(Guid authorId, CreateArticleDto request, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(request.ExtraData))
        {
            try
            {
                using var _ = JsonDocument.Parse(request.ExtraData);
            }
            catch (JsonException)
            {
                throw new ArgumentException("Поле extraData должно содержать корректный JSON.");
            }
        }

        var now = DateTime.UtcNow;

        var article = new Article
        {
            AuthorId = authorId,
            Title = request.Title.Trim(),
            Content = request.Content,
            ExtraData = string.IsNullOrWhiteSpace(request.ExtraData) ? null : request.ExtraData,
            Status = ArticleStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Articles.Add(article);
        await db.SaveChangesAsync(ct);

        return new GetArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Status = article.Status.ToString(),
            UpdatedAt = article.UpdatedAt
        };
    }
}