using LicenseService.Domain.Common;

namespace LicenseService.Domain.Enums;

public enum LicenseStatus
{
    Active = 1,
    Expired = 2,
    Suspended = 3,
    Revoked = 4,
    Pending = 5
}

public enum LicenseType
{
    Individual = 1,
    Enterprise = 2,
    Trial = 3,
    Academic = 4,
    NonProfit = 5
}
