using Licensify.Shared.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> LogoutAsync(string refreshToken);
    Task<bool> RevokeAllTokensAsync(Guid userId);
}

public interface ITokenService
{
    string GenerateAccessToken(UserDto user);
    string GenerateRefreshToken();
    bool ValidateAccessToken(string token);
}

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<bool> DeleteUserAsync(Guid id);
}
