using Domain.Enums;
using Domain.ServiceContracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SematecCC.Infra;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public int? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        return claim != null &&
               int.TryParse(claim.Value, out int userId)
            ? userId
            : null;
    }

    public string? GetCurrentUserName()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        return httpContext.User.Identity?.Name;
    }

    public bool? IsUserAdmin()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        return httpContext.User.IsInRole("admin");
    }

    public string GetRoleName()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return "";

        if (httpContext.User.IsInRole("admin"))
            return "admin";

        if (httpContext.User.IsInRole("companyAdmin"))
            return "companyAdmin";

        if (httpContext.User.IsInRole("companyUser"))
            return "companyUser";

        return "";
    }

    public int? GetCompanyId()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var claim = httpContext.User.FindFirst("CompanyId");

        return claim != null &&
               int.TryParse(claim.Value, out int companyId)
            ? companyId
            : null;
    }

    public int? GetCurrentRole1Id()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var roleClaim = httpContext.User.FindFirst("Role1Id");

        return roleClaim != null &&
               int.TryParse(roleClaim.Value, out int role1Id)
            ? role1Id
            : null;
    }

    public SortedSet<string> GetCurrentUserPermissions()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return new SortedSet<string>();

        return new SortedSet<string>(
            httpContext.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
        );
    }

    public bool HasPermission(UserPermissionsEnum permission)
    {
        // به جای IsUserAdmin().Value
        if (IsUserAdmin() == true)
            return true;

        return _httpContextAccessor.HttpContext?.User
            .HasClaim("Permission", permission.ToString()) ?? false;
    }

    public (bool? IsAdmin, int? CurrentUserId) GetAuthenticationIsAdminCurrentUserId()
    {
        return (
            IsUserAdmin(),
            GetCurrentUserId()
        );
    }

    public (string roleName, int? currentUserId) GetAuthenticationRoleNameUserId()
    {
        return (
            GetRoleName(),
            GetCurrentUserId()
        );
    }
}