namespace Licensify.Shared.DTOs;

public record LicenseDto(
    Guid Id,
    string LicenseKey,
    Guid ProductId,
    string ProductName,
    DateTime ExpiresAt,
    int MaxDevices,
    string? MaxIpAddresses,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateLicenseRequest(
    Guid ProductId,
    DateTime ExpiresAt,
    int MaxDevices = 1,
    string? MaxIpAddresses = null
);

public record ValidateLicenseRequest(
    string LicenseKey,
    string? DeviceId = null,
    string? IpAddress = null
);

public record ValidateLicenseResponse(
    bool IsValid,
    string? ErrorMessage = null,
    LicenseDto? License = null,
    DateTime? ExpiresAt = null
);

public record LicenseUsageDto(
    Guid Id,
    Guid LicenseId,
    string? DeviceId,
    string? IpAddress,
    DateTime UsedAt
);

public record RevokeLicenseRequest(
    Guid LicenseId,
    string Reason
);
