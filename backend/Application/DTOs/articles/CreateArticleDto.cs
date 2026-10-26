using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.articles;

public class CreateArticleDto
{
    [Required(ErrorMessage = "Название статьи обязательно для заполнения")]
    [MaxLength(200, ErrorMessage = "Название не может быть длиннее 200 символов")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Содержание статьи обязательно для заполнения")]
    public string Content { get; set; } = string.Empty;

    public string? ExtraData { get; set; }
}