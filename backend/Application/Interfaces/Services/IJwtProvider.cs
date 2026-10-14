
using Domain.Entities;


namespace Application.Interfaces.Services;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}