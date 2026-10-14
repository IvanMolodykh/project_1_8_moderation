using Application.DTOs.auth;
using Application.DTOs.auth.Tokens;


namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);

    Task<TokensAcReDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default);
    Task LogoutAsync(LogoutRequestDto request, CancellationToken ct = default);
}
