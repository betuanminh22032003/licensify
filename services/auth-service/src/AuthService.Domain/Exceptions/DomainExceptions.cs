namespace AuthService.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string email) 
        : base($"User with email '{email}' was not found") { }
    
    public UserNotFoundException(Guid userId) 
        : base($"User with ID '{userId}' was not found") { }
}

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() 
        : base("Invalid email or password") { }
}

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email) 
        : base($"User with email '{email}' already exists") { }
}

public class InactiveUserException : DomainException
{
    public InactiveUserException() 
        : base("User account is inactive") { }
}

public class InvalidRefreshTokenException : DomainException
{
    public InvalidRefreshTokenException() 
        : base("Invalid or expired refresh token") { }
}

public class TokenExpiredException : DomainException
{
    public TokenExpiredException() 
        : base("Token has expired") { }
}
