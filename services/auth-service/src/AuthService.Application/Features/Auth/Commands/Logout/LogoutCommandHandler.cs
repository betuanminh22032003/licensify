using MediatR;
using Microsoft.Extensions.Logging;
using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Repositories;

namespace AuthService.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutCommandResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<LogoutCommandResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return new LogoutCommandResult
                {
                    IsSuccess = false,
                    Message = "Refresh token is required"
                };
            }

            // Find and revoke the refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            
            if (refreshToken != null && !refreshToken.IsRevoked)
            {
                refreshToken.Revoke();
                _refreshTokenRepository.Update(refreshToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("User logged out successfully, refresh token revoked");
            }
            else
            {
                _logger.LogWarning("Refresh token not found or already revoked during logout");
            }

            // Always return success for logout operations to prevent token enumeration
            return new LogoutCommandResult
            {
                IsSuccess = true,
                Message = "Logged out successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return new LogoutCommandResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred during logout"
            };
        }
    }
}
