using Application.DTOs.auth.Tokens;

namespace Application.DTOs.auth;

public class RegisterResponseDto
{
    public TokensAcReDto Tokens { get; set; } = null!;
}
