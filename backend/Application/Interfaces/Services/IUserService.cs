using Application.DTOs.auth;


namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
}
