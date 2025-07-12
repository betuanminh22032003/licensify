using MediatR;

namespace AuthService.Application.Features.User.Commands.Delete;

public class DeleteUserCommand : IRequest<DeleteUserCommandResult>
{
    public int Id { get; set; }
}

public class DeleteUserCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
