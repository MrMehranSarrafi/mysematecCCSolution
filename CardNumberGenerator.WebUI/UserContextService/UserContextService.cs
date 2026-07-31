//using CardNoGenerator.Core;
//using System.Security.Claims;

//namespace CardNoGenerator.WebUI.Models
//{
//    public class UserContextService : IUserContextService
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        public UserContextService(IHttpContextAccessor httpContextAccessor)
//        {
//            _httpContextAccessor = httpContextAccessor;
//        }
//        public int? GetCurrentUserId()
//        {
//            if (!IsAuthenticated())
//                return null;
//            var userId = _httpContextAccessor.HttpContext?
//                            .User?
//                            .FindFirst(ClaimTypes.NameIdentifier)?
//                            .Value;
//            return userId != null ? int.Parse( userId ): null;
//        }

//        public string?  GetCurrentUserName()
//        {
//            if (!IsAuthenticated())
//                return null;
//            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
//        }

//        public bool IsAuthenticated()
//        {
//            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
//        }
//        public bool? IsUserAdmin()
//        {
//            if (!IsAuthenticated())
//                return null; 
//            var user = _httpContextAccessor.HttpContext?.User;
//            if (user == null || !user.Identity!.IsAuthenticated)
//                return null;//لاگین نکرده

//            return user.IsInRole("admin");//از کوکی که کلاینت فرستاده به سرور میخونه و میاره به حافظه
//        }


//        public  int? GetCompanyId()//از کوکی می خواند نه از دیتابیس
//        {
//            if (!IsAuthenticated())
//                return null;
//            var user = _httpContextAccessor.HttpContext?.User;
//            if (user== null || user.Identity == null || !user.Identity.IsAuthenticated)
//                return null;

//            // جستجو در Claimها بر اساس Type
//            var claim = user.FindFirst("CompanyId");

//            if (claim != null && int.TryParse(claim.Value, out int companyId))
//            {
//                return companyId;
//            }

//            return null;
//        }
//    }
//}
using CardNoGenerator.Core;
using System.Security.Claims;
namespace CardNoGenerator.WebUI;

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
        if (httpContext == null || !httpContext.User?.Identity?.IsAuthenticated == true)
            return null;
        var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out int userId) ? userId : null;
    }
    
    public string? GetCurrentUserName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User?.Identity?.IsAuthenticated == true)
            return null;
        return httpContext.User.Identity.Name;
    }
    public bool? IsUserAdmin()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User?.Identity?.IsAuthenticated == true)
            return null;
        return httpContext.User.IsInRole("admin");//companyAdmin and companyUser را هم اضافه کردم
    }
    public string GetRoleName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User?.Identity?.IsAuthenticated == true)
            return "";
        if (httpContext.User.IsInRole("admin"))
            return "admin";
        else if (httpContext.User.IsInRole("companyAdmin"))
            return "companyAdmin";
        else if (httpContext.User.IsInRole("companyUser"))
            return "companyUser";
        return "";
    }
    public int? GetCompanyId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User?.Identity?.IsAuthenticated == true)
            return null;
        var claim = httpContext.User.FindFirst("CompanyId");
        return claim != null && int.TryParse(claim.Value, out int companyId) ? companyId : null;
    }
    public int? GetCurrentRole1Id()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        // بررسی امنیت و احراز هویت
        if (httpContext == null || httpContext.User?.Identity?.IsAuthenticated != true)
            return null;

        // خواندن Role1Id از کوکی
        // توجه: کلیم را دقیقاً با همان نامی که در SetCompanyIdClaim ذخیره کردید ("Role1Id") بخوانید
        var roleClaim = httpContext.User.FindFirst("Role1Id");

        if (roleClaim != null && int.TryParse(roleClaim.Value, out int role1Id))
        {
            return role1Id;
        }

        return null;
    }
    public SortedSet<string> GetCurrentUserPermissions()
    {
        return new SortedSet<string>(
            _httpContextAccessor.HttpContext!.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
        );
    }
    public bool HasPermission(UserPermissionsEnum permission)
    {
        return IsUserAdmin().Value|| (_httpContextAccessor.HttpContext?.User
            .HasClaim("Permission", permission.ToString()) ?? false);
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
