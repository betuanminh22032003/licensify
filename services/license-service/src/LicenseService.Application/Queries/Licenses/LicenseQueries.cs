using MediatR;
using LicenseService.Domain.Enums;

namespace LicenseService.Application.Queries.Licenses;

public record GetLicenseByIdQuery(Guid LicenseId) : IRequest<LicenseDto?>;

public record GetLicenseByKeyQuery(string LicenseKey) : IRequest<LicenseDto?>;

public record GetLicensesByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<LicenseDto>>;

public record GetLicensesByProductQuery(Guid ProductId) : IRequest<IEnumerable<LicenseDto>>;

public record GetLicensesByStatusQuery(LicenseStatus Status) : IRequest<IEnumerable<LicenseDto>>;

public record GetExpiringLicensesQuery(DateTime BeforeDate) : IRequest<IEnumerable<LicenseDto>>;

public record GetExpiredLicensesQuery() : IRequest<IEnumerable<LicenseDto>>;

public class LicenseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public LicenseType Type { get; set; }
    public LicenseStatus Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public int MaxUsers { get; set; }
    public int CurrentUsers { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
