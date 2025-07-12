using MediatR;
using AuthService.Domain.Entities;

namespace AuthService.Application.Features.User.Queries.Get;

public class GetUserQuery : IRequest<GetUserQueryResult>
{
    public int Id { get; set; }
}

public class GetUserQueryResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Domain.Entities.User? User { get; set; }
}
