using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenCommandResult>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
