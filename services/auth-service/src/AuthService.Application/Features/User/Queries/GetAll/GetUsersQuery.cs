using MediatR;
using AuthService.Domain.Entities;

namespace AuthService.Application.Features.User.Queries.GetAll;

public class GetUsersQuery : IRequest<GetUsersQueryResult>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUsersQueryResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<Domain.Entities.User> Users { get; set; } = Enumerable.Empty<Domain.Entities.User>();
    public int TotalCount { get; set; }
}
