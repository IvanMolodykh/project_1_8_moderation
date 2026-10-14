
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email обязателен для заполнения")]
    [EmailAddress(ErrorMessage = "Укажите корректный адрес электронной почты")]
    [MaxLength(255, ErrorMessage = "Email не может быть длиннее 255 символов")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен для заполнения")]
    [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
    [MaxLength(100, ErrorMessage = "Пароль не может быть длиннее 100 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Пароль должен содержать хотя бы одну заглавную букву, одну строчную букву, одну цифру и один специальный символ")]
    public string Password { get; set; } = string.Empty;

    // В базе стоит ограничение 100 символов
    [Required(ErrorMessage = "Имя для отображения обязательно")]
    [MinLength(2, ErrorMessage = "Имя должно содержать минимум 2 символа")]
    [MaxLength(100, ErrorMessage = "Имя не может быть длиннее 100 символов")]
    public string DisplayName { get; set; } = string.Empty;
}