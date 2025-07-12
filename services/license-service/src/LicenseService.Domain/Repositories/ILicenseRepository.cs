using LicenseService.Domain.Entities;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Enums;

namespace LicenseService.Domain.Repositories;

public interface ILicenseRepository
{
    Task<License?> GetByIdAsync(LicenseId id, CancellationToken cancellationToken = default);
    Task<License?> GetByLicenseKeyAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByProductIdAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByStatusAsync(LicenseStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetExpiringLicensesAsync(DateTime beforeDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetExpiredLicensesAsync(CancellationToken cancellationToken = default);
    Task<License> AddAsync(License license, CancellationToken cancellationToken = default);
    void Update(License license);
    void Delete(License license);
    Task<bool> ExistsByLicenseKeyAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default);
    Task<int> GetActiveLicenseCountByProductAsync(ProductId productId, CancellationToken cancellationToken = default);
}
