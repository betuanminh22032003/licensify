using System.Security.Claims;
using AuthService.Domain.Entities;

namespace AuthService.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    DateTime GetRefreshTokenExpiration();
    bool ValidateToken(string token);
}
