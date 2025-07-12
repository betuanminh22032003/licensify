using MediatR;
using AuthService.Domain.Entities;

namespace AuthService.Application.Features.User.Commands.Create;

public class CreateUserCommand : IRequest<CreateUserCommandResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateUserCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Domain.Entities.User? User { get; set; }
}
