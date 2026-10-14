using System.ComponentModel.DataAnnotations;

namespace Paper.Application.DTOs;

public sealed class CreateUserDto
{
    [Required(ErrorMessage = "Имя обязательно.")]
    [MaxLength(100, ErrorMessage = "Имя не длиннее 100 символов.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен.")]
    [MaxLength(256, ErrorMessage = "Email не длиннее 256 символов.")]
    [EmailAddress(ErrorMessage = "Некорректный email.")]
    public string Email { get; set; } = string.Empty;
}
