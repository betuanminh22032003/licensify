using MediatR;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Features.Role.Commands.Create;

public class CreateRoleCommand : IRequest<CreateRoleCommandResult>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class CreateRoleCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserRole? Role { get; set; }
}
