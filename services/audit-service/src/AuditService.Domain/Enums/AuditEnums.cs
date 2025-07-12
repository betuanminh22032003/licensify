namespace AuditService.Domain.Enums;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    Activate = 6,
    Deactivate = 7,
    Suspend = 8,
    Revoke = 9,
    Validate = 10,
    Extend = 11,
    Access = 12,
    Export = 13,
    Import = 14,
    Configure = 15
}

public enum AuditSeverity
{
    Information = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

public enum EntityType
{
    User = 1,
    Product = 2,
    License = 3,
    Customer = 4,
    System = 5,
    Configuration = 6
}
