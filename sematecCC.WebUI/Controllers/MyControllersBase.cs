using SematecCC.Core;
using SematecCC.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace SematecCC.WebUI.Controllers;


public class MyControllersBase : Controller
{
    protected readonly IUserContextService _userContextService;
    protected readonly CompanyService _companyService;
    public MyControllersBase(IUserContextService userContext, CompanyService companyService)
    {
        _userContextService = userContext;
        _companyService = companyService;
    }
    public MyControllersBase(IUserContextService userContext)
    {
        _userContextService = userContext;
    }
    protected (bool? IsAdmin, int? CompanyId, int? UserId) GetAuthenticationInfo()
    {
        return (
            _userContextService.IsUserAdmin(),
            _userContextService.GetCompanyId(),
            _userContextService.GetCurrentUserId()
        );
    }
    protected (bool? IsAdmin, int? CompanyId, int? UserId, string RoleName) GetAuthenticationFullInfo()
    {
        return (
            _userContextService.IsUserAdmin(),
            _userContextService.GetCompanyId(),
            _userContextService.GetCurrentUserId(),
            _userContextService.GetRoleName()
        );
    }
    protected int? GetCurrentUserId()
    {
        return _userContextService.GetCurrentUserId();
    }
    protected bool? IsUserAdmin()
    {
        return _userContextService.IsUserAdmin();
    }
    protected async Task<List<ComboItemsList>> GetComboAllowedCompaniesSelectList(bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        return await _companyService.GetComboAllowedCompanies(isAdmin, currentCompanyId, currentUserId);

    }
    protected SortedSet<string> GetCurrentUserPermissions()
    {
        return _userContextService.GetCurrentUserPermissions();
    }
    protected bool HasPermission(UserPermissionsEnum action)
    {
        //return GetCurrentUserPermissions().Contains(action.ToString());  هر دفعه آرایه نساز
        //مستقیم بخوان از Claims:
        return IsUserAdmin().Value || _userContextService.HasPermission(action);
    }
    protected int? GetCompanyId()
    {
        return _userContextService.GetCompanyId();
    }


}
