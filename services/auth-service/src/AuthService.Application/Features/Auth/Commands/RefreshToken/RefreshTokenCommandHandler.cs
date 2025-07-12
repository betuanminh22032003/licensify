using MediatR;
using Microsoft.Extensions.Logging;
using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Repositories;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            
            if (refreshToken == null || !refreshToken.IsValid)
            {
                _logger.LogWarning("Refresh token not found or invalid");
                return new RefreshTokenCommandResult
                {
                    IsSuccess = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            // Get the user
            var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
            
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User not found or inactive for refresh token");
                return new RefreshTokenCommandResult
                {
                    IsSuccess = false,
                    Message = "User not found or account is inactive"
                };
            }

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            var newRefreshTokenExpiration = _jwtTokenService.GetRefreshTokenExpiration();

            // Revoke old refresh token
            refreshToken.Revoke();
            _refreshTokenRepository.Update(refreshToken);

            // Create new refresh token
            var newRefreshToken = user.CreateRefreshToken(newRefreshTokenValue, newRefreshTokenExpiration);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);

            return new RefreshTokenCommandResult
            {
                IsSuccess = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return new RefreshTokenCommandResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred while refreshing token"
            };
        }
    }
}
