using SematecCC.Core;
using System.Security.Claims;

namespace SematecCC.WebUI;

public static class ClaimsPrincipalExtensions
{
    public static class CustomClaimTypes
    {
        public const string Permission = "Permission";
        public const string CompanyId = "CompanyId";
        public const string Role1Id = "Role1Id";
        public const string FullName = "FullName";
    }
    public static bool HasPermission(this ClaimsPrincipal user, UserPermissionsEnum permission)
    {
        if (user == null || !user.Identity.IsAuthenticated)
            return false;
        return user.IsInRole("admin")|| user.HasClaim(/*"Permission"*/CustomClaimTypes.Permission, permission.ToString());
    }
}
