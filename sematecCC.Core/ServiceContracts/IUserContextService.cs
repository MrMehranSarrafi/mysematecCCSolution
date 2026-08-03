namespace CardNoGenerator.Core;//Related to Web UI, Not Repo. Core is dependant of both Repo and UI

public interface IUserContextService
{
     int? GetCurrentUserId();
     string? GetCurrentUserName();
     bool IsAuthenticated();
     bool? IsUserAdmin();
     int? GetCompanyId();
     int? GetCurrentRole1Id();
     string GetRoleName();
     SortedSet<string> GetCurrentUserPermissions();
     bool HasPermission(UserPermissionsEnum permission);
     (bool? IsAdmin, int? CurrentUserId) GetAuthenticationIsAdminCurrentUserId();
    (string roleName, int? currentUserId) GetAuthenticationRoleNameUserId();
     

}
