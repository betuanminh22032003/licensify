using System.Security.Claims;

namespace Licensify.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public static string GetUserRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.GetUserRole().Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsDeveloper(this ClaimsPrincipal principal)
    {
        return principal.GetUserRole().Equals("Developer", StringComparison.OrdinalIgnoreCase);
    }
}
