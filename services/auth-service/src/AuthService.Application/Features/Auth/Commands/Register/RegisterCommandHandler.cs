using MediatR;
using Microsoft.Extensions.Logging;
using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Entities;
using AuthService.Domain.Events;

namespace AuthService.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RegisterCommandResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate email format
            var email = Email.Create(request.Email);
            
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: User already exists with email {Email}", request.Email);
                return new RegisterCommandResult
                {
                    IsSuccess = false,
                    Message = "User with this email already exists"
                };
            }

            // Validate password strength (basic validation)
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return new RegisterCommandResult
                {
                    IsSuccess = false,
                    Message = "Password must be at least 6 characters long"
                };
            }

            // Validate names
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return new RegisterCommandResult
                {
                    IsSuccess = false,
                    Message = "First name and last name are required"
                };
            }

            // Create user role (default to Developer)
            var userRole = UserRole.Developer;

            // Create user entity
            var user = Domain.Entities.User.Create(
                email,
                request.Email, // Use email as username for now
                request.Password,
                request.FirstName.Trim(),
                request.LastName.Trim(),
                userRole);

            // Add user to repository
            await _userRepository.AddAsync(user, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User registered successfully with email {Email}", request.Email);

            return new RegisterCommandResult
            {
                IsSuccess = true,
                Message = "User registered successfully",
                User = user
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Registration failed: Invalid input - {Message}", ex.Message);
            return new RegisterCommandResult
            {
                IsSuccess = false,
                Message = "Invalid input data: " + ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed: Unexpected error for email {Email}", request.Email);
            return new RegisterCommandResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred during registration"
            };
        }
    }
}
