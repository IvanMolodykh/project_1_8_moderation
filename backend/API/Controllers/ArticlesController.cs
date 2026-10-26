using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs.articles;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/articles")]
[Authorize]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<List<GetArticleDto>>> GetMine(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "Не удалось определить пользователя." });
        }

        var articles = await articleService.GetMyArticlesAsync(userId.Value, ct);
        return Ok(articles);
    }

    [HttpPost]
    public async Task<ActionResult<GetArticleDto>> Create([FromBody] CreateArticleDto request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "Не удалось определить пользователя." });
        }

        try
        {
            var article = await articleService.CreateArticleAsync(userId.Value, request, ct);
            return StatusCode(StatusCodes.Status201Created, article);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id) ? id : null;
    }
}