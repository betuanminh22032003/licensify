using MediatR;
using AuthService.Domain.Entities;

namespace AuthService.Application.Features.User.Commands.Update;

public class UpdateUserCommand : IRequest<UpdateUserCommandResult>
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateUserCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Domain.Entities.User? User { get; set; }
}
