using Application.DTOs.articles;

namespace Application.Interfaces.Services;

public interface IArticleService
{
    Task<List<GetArticleDto>> GetMyArticlesAsync(Guid authorId, CancellationToken ct = default);
    Task<GetArticleDto> CreateArticleAsync(Guid authorId, CreateArticleDto request, CancellationToken ct = default);
}