using Application.DTOs.auth.Tokens;

namespace Application.DTOs.auth;

public class LoginResponseDto
{
    public TokensAcReDto Tokens { get; set; } = null!;
}
