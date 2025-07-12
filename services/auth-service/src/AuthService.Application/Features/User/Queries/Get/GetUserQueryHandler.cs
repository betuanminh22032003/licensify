using MediatR;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Repositories;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Features.User.Queries.Get;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserQueryResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserQueryResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = UserId.Create(request.Id);
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", request.Id);
                return new GetUserQueryResult
                {
                    IsSuccess = false,
                    Message = "User not found",
                    User = null
                };
            }

            _logger.LogInformation("User retrieved successfully: {UserId}", request.Id);
            return new GetUserQueryResult
            {
                IsSuccess = true,
                Message = "User retrieved successfully",
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", request.Id);
            return new GetUserQueryResult
            {
                IsSuccess = false,
                Message = "An error occurred while retrieving user",
                User = null
            };
        }
    }
}
