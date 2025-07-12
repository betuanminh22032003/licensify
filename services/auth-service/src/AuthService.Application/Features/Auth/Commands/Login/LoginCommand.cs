using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Login;

public record LoginCommand : IRequest<LoginCommandResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public class LoginCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public LoginResponse? Data { get; set; }
}

public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
