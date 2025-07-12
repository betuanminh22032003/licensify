using MediatR;

namespace LicenseService.Application.Commands.Licenses;

public record ActivateLicenseCommand(
    Guid LicenseId
) : IRequest<bool>;

public record SuspendLicenseCommand(
    Guid LicenseId,
    string Reason
) : IRequest<bool>;

public record RevokeLicenseCommand(
    Guid LicenseId,
    string Reason
) : IRequest<bool>;

public record ExtendLicenseCommand(
    Guid LicenseId,
    DateTime NewExpirationDate
) : IRequest<bool>;

public record ValidateLicenseCommand(
    string LicenseKey
) : IRequest<ValidateLicenseResponse>;

public record ValidateLicenseResponse(
    bool IsValid,
    string? Reason = null,
    DateTime? ExpiresAt = null,
    int? RemainingUsers = null
);
