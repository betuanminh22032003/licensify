using MediatR;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Enums;

namespace LicenseService.Application.Commands.Licenses;

public record CreateLicenseCommand(
    Guid ProductId,
    Guid CustomerId,
    LicenseType Type,
    DateTime ExpiresAt,
    int MaxUsers = 1,
    string? Notes = null
) : IRequest<CreateLicenseResponse>;

public record CreateLicenseResponse(
    Guid LicenseId,
    string LicenseKey,
    DateTime IssuedAt,
    DateTime ExpiresAt
);
