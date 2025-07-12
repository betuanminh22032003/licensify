namespace LicenseService.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class LicenseNotFoundException : DomainException
{
    public LicenseNotFoundException(string licenseKey) : base($"License with key '{licenseKey}' was not found") { }
    public LicenseNotFoundException(Guid licenseId) : base($"License with ID '{licenseId}' was not found") { }
}

public class LicenseAlreadyExistsException : DomainException
{
    public LicenseAlreadyExistsException(string licenseKey) : base($"License with key '{licenseKey}' already exists") { }
}

public class LicenseExpiredException : DomainException
{
    public LicenseExpiredException(string licenseKey) : base($"License '{licenseKey}' has expired") { }
}

public class LicenseNotActiveException : DomainException
{
    public LicenseNotActiveException(string licenseKey) : base($"License '{licenseKey}' is not active") { }
}

public class LicenseUserLimitExceededException : DomainException
{
    public LicenseUserLimitExceededException(string licenseKey, int maxUsers) 
        : base($"License '{licenseKey}' has reached its maximum user limit of {maxUsers}") { }
}

public class InvalidLicenseOperationException : DomainException
{
    public InvalidLicenseOperationException(string operation, string reason) 
        : base($"Cannot perform operation '{operation}': {reason}") { }
}
