// Файл: Application/Services/UserService.cs
using Application.DTOs.auth;

using Application.Interfaces;
using Application.Interfaces.Services;
using BCrypt.Net;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;



namespace Application.Services;

public class UserService(IAppDbContext db, IJwtProvider jwtProvider) : IUserService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
        {
            throw new ArgumentException("Пользователь с таким email уже существует.");
        }

        var user = new User
        {
            Email = email,
            DisplayName = request.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password)
        };

        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshTokenString = jwtProvider.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshTokenString),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user // EF Core автоматически свяжет их при сохранении
        };

        db.Users.Add(user);
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(ct);

        return new RegisterResponseDto
        {
            Tokens = new DTOs.auth.Tokens.TokensAcReDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            }
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный email или пароль.");
        }

        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshTokenString = jwtProvider.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshTokenString),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(ct);

        return new LoginResponseDto
        {
            Tokens = new DTOs.auth.Tokens.TokensAcReDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            }
        };
    }

    // Вспомогательный метод для хеширования SHA-256 (согласно ТЗ к БД)
    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}