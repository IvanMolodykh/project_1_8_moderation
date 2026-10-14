using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.auth.Tokens;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh Token обязателен")]
    public string RefreshToken { get; set; } = string.Empty;
}