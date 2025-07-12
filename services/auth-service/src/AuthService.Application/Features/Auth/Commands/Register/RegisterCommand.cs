using MediatR;
using AuthService.Domain.Entities;

namespace AuthService.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterCommandResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Domain.Entities.User? User { get; set; }
}
