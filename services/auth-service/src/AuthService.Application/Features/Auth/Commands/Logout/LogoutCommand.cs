using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<LogoutCommandResult>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LogoutCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
