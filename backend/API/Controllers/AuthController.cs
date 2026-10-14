// Файл: API/Controllers/AuthController.cs
using Application.DTOs.auth;
using Application.DTOs.auth.Tokens;
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase
{


    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await userService.RegisterAsync(request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await userService.LoginAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }


    [HttpPost("refresh")]
    public async Task<ActionResult<TokensAcReDto>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await userService.RefreshTokenAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Возвращаем 401, чтобы фронтенд понял, что токен умер и нужно перекинуть юзера на страницу логина
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken ct)
    {
        await userService.LogoutAsync(request, ct);

        // При логауте обычно просто возвращаем 200 OK без данных
        return Ok(new { message = "Вы успешно вышли из системы." });
    }
}