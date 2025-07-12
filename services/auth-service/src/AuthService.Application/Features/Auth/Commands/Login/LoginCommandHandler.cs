using MediatR;
using Microsoft.Extensions.Logging;
using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Exceptions;
using AuthService.Domain.Events;

namespace AuthService.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHashingService passwordHashingService,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHashingService = passwordHashingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate email format
            var email = Email.Create(request.Email);
            
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                return new LoginCommandResult
                {
                    IsSuccess = false,
                    Message = "Invalid email or password"
                };
            }

            // Verify password
            if (!user.VerifyPassword(request.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
                return new LoginCommandResult
                {
                    IsSuccess = false,
                    Message = "Invalid email or password"
                };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: User account is inactive for email {Email}", request.Email);
                return new LoginCommandResult
                {
                    IsSuccess = false,
                    Message = "Account is inactive. Please contact administrator."
                };
            }

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiration = _jwtTokenService.GetRefreshTokenExpiration();

            // Create refresh token entity
            var refreshToken = user.CreateRefreshToken(refreshTokenValue, refreshTokenExpiration);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            // Update last login
            user.UpdateLastLogin();
            
            // Add domain event
            user.AddUserLoggedInEvent(request.IpAddress ?? "Unknown");

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            return new LoginCommandResult
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenValue,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Based on JWT config
                    User = new UserDto
                    {
                        Id = user.Id.Value,
                        Email = user.Email.Value,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role.Value,
                        CreatedAt = user.CreatedAt
                    }
                }
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Login failed: Invalid input - {Message}", ex.Message);
            return new LoginCommandResult
            {
                IsSuccess = false,
                Message = "Invalid input data"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed: Unexpected error for email {Email}", request.Email);
            return new LoginCommandResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred"
            };
        }
    }
}
