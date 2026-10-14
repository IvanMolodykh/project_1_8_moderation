// Файл: Application/Services/UserService.cs
using Application.DTOs.auth;
using Application.DTOs.auth.Tokens;
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


    public async Task<TokensAcReDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
    {
        var tokenHash = HashToken(request.RefreshToken);

        // Ищем токен в БД вместе с пользователем (Include нужен, чтобы потом сгенерировать Access-токен)
        var existingToken = await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        // Проверяем: существует ли токен, не отозван ли он и не истёк ли срок годности
        if (existingToken == null || existingToken.RevokedAt != null || existingToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Невалидный или просроченный Refresh Token.");
        }

        // Помечаем старый токен как отозванный (Token Rotation)
        existingToken.RevokedAt = DateTime.UtcNow;

        // Генерируем новую пару токенов
        var newAccessToken = jwtProvider.GenerateAccessToken(existingToken.User);
        var newRefreshTokenString = jwtProvider.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = existingToken.User.Id,
            TokenHash = HashToken(newRefreshTokenString),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync(ct);

        return new TokensAcReDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString
        };
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken ct = default)
    {
        var tokenHash = HashToken(request.RefreshToken);

        // Ищем токен
        var existingToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        // Если токен найден и ещё не отозван — отзываем его
        if (existingToken != null && existingToken.RevokedAt == null)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

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
            User = user
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

    // Вспомогательный метод для хеширования SHA-256
    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}