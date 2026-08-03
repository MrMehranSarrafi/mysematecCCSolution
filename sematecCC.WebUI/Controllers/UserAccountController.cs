using SematecCC.Core;
using SematecCC.Core.Services;
using SematecCC.WebUI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SematecCC.WebUI.Controllers;

public class UserAccountController : MyControllersBase
{
    private readonly /*I*/UserService _userService;
    //private readonly CompanyService _companyService;
    //private readonly IUserContextService _userContext;
    public UserAccountController(/*I*/UserService user, IUserContextService userContextService, CompanyService companyService) : base(userContextService, companyService)
    {
        _userService = user;
    }
    #region Permissions


    #region مجوزها    کاربر
    [Authorize(Roles = "admin, companyAdmin")]
    public async Task<IActionResult> UserPermissions(int userId, string username = "")
    {
        (bool? IsAdmin, int? CompanyId, int? currentUserId, string currentRoleName) = GetAuthenticationFullInfo();
        List<PermissionDto> list = await _userService.GetUserPermissions(userId, currentUserId, currentRoleName);
        ViewBag.UserId = userId;
        ViewBag.Username = username;
        ViewBag.CurrentUserId = currentUserId;
        return View("Permissions/UserPermissions", list);

    }
    [Authorize(Roles = "admin, companyAdmin")]
    [HttpPost]
    public async Task<IActionResult> UserPermissions([FromBody] PermissionRequest request)
    {
        int userId = request.UserId;
        int[] permissions = request.PermissionList ?? new int[0];

        var result = await _userService.SetUserPermissions(userId, permissions);
        //return Json(result);
        //return RedirectToAction(nameof(Index));//توجه کن این اکشن با ایجکس فراخوانی شده
        //  خود AJAX آن Redirect را دنبال می‌کند و محتوای Index را دریافت می‌کند.
        //NOTE: اما آن HTML فقط به عنوان Response به کد JavaScript برمی‌گردد؛ مرورگر به صفحه Index نمی‌رود
        return Json(new
        {
            success = result.Success,
            type =  result.Success ?  2:1,//"Success"=2, Error=1,
            backUrl = Url.Action(nameof(Index))
        });

    }
    [Authorize(Roles = "admin, companyAdmin")]
    public async Task<IActionResult> UserPermissiongroups(int userId, string username = "")
    {
        (bool? IsAdmin, int? CompanyId, int? currentUserId, string currentRoleName) = GetAuthenticationFullInfo();
        List<PermissiongroupDto> list = await _userService.GetUserPermissiongroups(userId, currentUserId, currentRoleName);
        ViewBag.UserId = userId;
        ViewBag.Username = username;


        return View("Permissions/UserPermissiongroups", list);

    }
    [Authorize(Roles = "admin, companyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserPermissiongroups(int userId, int[] permissiongroupIds)
    {
        permissiongroupIds ??= new int[0];
        var result = await _userService.SetUserPermissiongroups(userId, permissiongroupIds);
        //if (result.Success)
        //    TempData["SuccessMessage"] = result.Message;
        //else
        //    @TempData["ErrorMessage"] = result.Message;
        //return RedirectToAction("Permissions/UserPermissiongroups");
        return RedirectToAction(nameof(Index));

    }
    #endregion
    #region گروه های مجوز ها
    [Authorize(Roles = "admin, companyAdmin")]
    public async Task<IActionResult> PermissiongroupPermissions(int permissiongroupId , string permissiongroupName = "")
    {
        List<PermissionDto> list = await _userService.GetPermissiongroupPermissions(permissiongroupId);
        ViewBag.PermissiongroupId = permissiongroupId;
        ViewBag.PermissiongroupName = permissiongroupName;
        return View("Permissions/PermissiongroupPermissions", list);
    }

    [HttpPost]
    public async Task<IActionResult> PermissiongroupPermissions([FromBody] PermissionRequest request)
    {
        int groupId = request.PermissiongroupId;
        int[] permissions = request.PermissionList ?? new int[0];
        var result = await _userService.SetPermissiongroupPermissions(groupId, permissions);

        //return Json(result);
        //return RedirectToAction(nameof(PermissionGroupIndex));
        return Json(new
        {
            success = result.Success,
            type = result.Success ? 2 : 1,//"Success"=2, Error=1,
            backUrl = "UserAccount/PermissiongroupPermissions\""
        });
    }
    #endregion
    [Authorize(Roles = "admin")]/*, companyAdmin*/
    public async Task<IActionResult> PermissionGroupIndex()
    {
        (string roleName, int? currentUserId) = _userContextService.GetAuthenticationRoleNameUserId();
        var list = await _userService.GetPermissionGroups(roleName, currentUserId);
        return View("Permissions/PermissionGroupIndex", list);
    }
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PermissionGroupEdit(int id)
    {
        var permissionGroup = await _userService.GetPermissionGroupById(id);
        return View("Permissions/PermissionGroupEdit", permissionGroup);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermissionGroupEdit(int Id, Permissiongroup perGroup)
    {
        if (!ModelState.IsValid)
        {
            return View(perGroup);
        }

        OperationResultDto operationresult = await _userService.PermissionGroupEdit(perGroup);
        if (operationresult.Success)
        {
            //TempData["Message"] = operationresult.Message;
            TempData["SuccessMessage"] = operationresult.Message;
            return RedirectToAction(nameof(PermissionGroupIndex));
        }


        ModelState.AddModelError(operationresult.PropertyName, operationresult.Message);

        ViewBag.Message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

        return View(perGroup);
    }
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PermissionGroupCreate()
    {

        return View("Permissions/PermissionGroupCreate");
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermissionGroupCreate(Permissiongroup permissionGroup)
    {
        if (!ModelState.IsValid)
        {
            return View(permissionGroup);
        }


        var result = await _userService.PermissionGroupCreateAsync(permissionGroup);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(PermissionGroupIndex));
        }
        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors);
        return View(permissionGroup);
    }
    #endregion
    [Authorize]
    public async Task<IActionResult> Index()
    {
        //var listpermissiontest = HasPermission(UserPermissionsEnum.UserAccountList);
           
        (bool? isAdmin, int? currentCompanyId, int? currentUserId, string roleName) = GetAuthenticationFullInfo();
        var users = await _userService.GetAllUsersAsync(isAdmin, currentCompanyId, currentUserId, roleName);
        ViewBag.IsAdmin = isAdmin == true ? true : false;
        //return PartialView(users);
        return View(users);
    }
    
    [Route("UserAccount/Register")]
    [Route("UserAccount/Create")]
    //[Authorize(Roles ="admin")]
    [Authorize]//ادمین کاربران شرکت ها را ایجاد کرده و او کاربران خودش را ایجاد می کند.
    public async Task<IActionResult> Create()
    {
        bool? isAdmin = IsUserAdmin();//_userContextService.IsUserAdmin();
        ViewBag.IsAdmin = isAdmin;
        List<ComboItemsList> Companies = await _companyService.GetComboCompanies();//.GetCompanyIDs();
        int CompanyId = await _userService.GetCompanyId(_userContextService.GetCurrentUserId());//می تونی از claim  بخونی که در کوکی ذخیره کرده ام
        ViewBag.CompanyId = CompanyId;

        if (isAdmin == false)
        {
            Companies = Companies.Where(c => c.Value == CompanyId.ToString()).ToList();
        }
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");
        return PartialView();
    }
    //[Authorize(Roles ="admin")] install Microsoft.AspNetCore.Identity first.
    [HttpPost]
    [Route("UserAccount/Register")]
    [Route("UserAccount/Create")]
    [ValidateAntiForgeryToken]//CSRF (Cross-Site Request Forgery)
    public async Task<IActionResult> Create(CreateUserViewModel userVM)
    {
        List<ComboItemsList> Companies = await _companyService.GetComboCompanies();//.GetCompanyIDs();            
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        if (isAdmin == false)
        {
            Companies = Companies.Where(c => c.Value == currentCompanyId.ToString()).ToList();
        }
        ViewBag.IsAdmin = isAdmin;
        ViewBag.Companies = new SelectList(Companies, "Value", "Text", userVM.CompanyId);
        ViewBag.CompanyId = currentCompanyId;

        if (!ModelState.IsValid)
        {
            return View(userVM);
        }
        var userDto = new EditUserDto
        {
            FirstName = userVM.FirstName,
            LastName = userVM.LastName,
            Email = userVM.Email,

            CompanyCode = userVM.CompanyCode,
            CompanyName = userVM.CompanyName,
            CompanyId = userVM.CompanyId,

            Description = userVM.Description,
            PhoneNumber = userVM.PhoneNumber,
            Telephone = userVM.Telephone,
            UserName = userVM.UserName,
            Password = userVM.Password,
            ConfirmPassword = userVM.ConfirmPassword
        };

        var result = await _userService.CreateUserAsync(userDto, isAdmin, currentCompanyId, currentUserId);
        if (result.Success)
        {
            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors);
        return View(userVM);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id != 1)
        {
            var result = await _userService.Delete(id);
            TempData["SuccessMessage"] = result.Message;
        }
        else
            TempData["ErrorMessage"] = "ادمین قابل حذف نمی باشد.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> DisableUser(int id)
    {
        int? currentUserId = GetCurrentUserId();
        var result = await _userService.DisableUser(id, currentUserId);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
            TempData["ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index));

    }
    public async Task<IActionResult> EnableUser(int id)
    {
        var result = await _userService.EnableUser(id);
        //TempData["Message"] = result.Message;
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));

    }
    [HttpGet]
    [Route("UserAccount/Login")]
    public IActionResult Login()
    {
        //https://localhost:44327/UserAccount/Login?ReturnUrl=%2F
        ////%2F : /      (%2F : کدگذاری شده (URL encoded) کاراکتر / است.)

        ViewBag.ReturnUrl = Request.Query["ReturnUrl"];
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel login, string? returnUrl = null)
    {
        OperationResultDto result = new OperationResultDto();

        if (ModelState.IsValid)
        {
            var loginDto = new LoginDto
            {
                PassWord = login.Password,
                UserName = login.UserName,
                RememberMe = login.RememberMe
            };

            result = await _userService.Login(loginDto);

            if (result.Success)
            {
                //var returnUrl = Response.Cookies["CardsAuthCookie"];
                // ✅ بررسی وجود ReturnUrl
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    // اگر آدرس برگشتی وجود دارد و مربوط به سایت خودمان است، به آنجا برو
                    return Redirect(returnUrl);
                }
                else
                {
                    // در غیر این صورت به صفحه اصلی برو
                    return RedirectToAction(nameof(HomeController.IndexNew), "Home");
                }
            }
        }

        ModelState.AddModelError("", result.Message);
        return View(login);
    }
    public async Task<IActionResult> Edit(int Id)
    {
        var userDto = await _userService.GetUserByIdAsync(Id);
        EditUserViewModel userUi = new EditUserViewModel();
        if (userDto != null)
        {
            userUi.Id = userDto.Id;
            userUi.FirstName = userDto.FirstName;
            userUi.LastName = userDto.LastName;
            userUi.PhoneNumber = userDto.PhoneNumber;
            userUi.Telephone = userDto.Telephone;
            userUi.CompanyId = userDto.CompanyId;
            userUi.CompanyCode = userDto.CompanyCode;
            userUi.CompanyName = userDto.CompanyName;
            userUi.UserName = userDto.UserName;
            userUi.Email = userDto.Email;
            userUi.Description = userDto.Description;
        }
        else
            return NotFound();

        bool? IsAdmin = _userContextService.IsUserAdmin();
        ViewBag.IsAdmin = IsAdmin;
        List<ComboItemsList> Companies = await _companyService.GetComboCompanies();
        int CompanyId = await _userService.GetCompanyId(_userContextService.GetCurrentUserId());
        ViewBag.CompanyId = CompanyId;

        if (IsAdmin == false)
        {
            Companies = Companies.Where(c => c.Value == CompanyId.ToString()).ToList();
        }
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");

        return View(userUi);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, EditUserViewModel userVM)
    {
        if (!string.IsNullOrWhiteSpace(userVM.Password))
        {

            if (userVM.Password?.Trim() != userVM.ConfirmPassword?.Trim())
            {
                ModelState.AddModelError("ConfirmPassword", "رمز های عبور وارد شده یکسان نیستند.");
                return View(userVM);
            }
        }
        if (!ModelState.IsValid)
        {
            return View(userVM);
        }
        var userDto = new EditUserDto
        {
            Id = Id,
            FirstName = userVM.FirstName,
            LastName = userVM.LastName,
            Email = userVM.Email,
            CompanyId = userVM.CompanyId,
            CompanyCode = userVM.CompanyCode,
            CompanyName = userVM.CompanyName,
            Description = userVM.Description,
            PhoneNumber = userVM.PhoneNumber,
            Telephone = userVM.Telephone,
            UserName = userVM.UserName,
            Password = userVM.Password,
            ConfirmPassword = userVM.ConfirmPassword
        };
        OperationResultDto operationResult = await _userService.UpdateAsync(userDto);
        if (operationResult.Success)
        {
            TempData["SuccessMessage"] = operationResult.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(operationResult.PropertyName, operationResult.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors);

        return View(userVM);
    }
    public async Task<IActionResult> Search(string searchItems)
    {
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();

        var users = await _userService.GetUsersAsync(searchItems, isAdmin, currentCompanyId, currentUserId);
        ViewBag.IsAdmin = isAdmin;
        return PartialView("_UserRowsPartialView", users);
    }

    public async Task<IActionResult> Search1(string searchItems)
    {
        var users = await _userService.GetUsersAsync(searchItems);
        //return View("Index2",users);
        return Json(users);
    }


    public async Task<IActionResult> ChangePassword(int Id, string UserName)
    {
        var p = new ChangePasswordDto()
        {
            Id = Id,
            UserName = UserName
        };
        return View(p);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto PasswordInfo)
    {
        if (!ModelState.IsValid)
        {
            return View(PasswordInfo);
        }

        var result = await _userService.ChangePassword(PasswordInfo);
        if (result.Success)
        {
            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        return View(PasswordInfo);
    }
    //[Authorize]//Should be logged in
    public async Task<IActionResult> LogOut(string returnUrl = null, string ComeFrom = "")
    {
        bool result = await _userService.LogOut();
        if (result)
        {
            // پاک کردن کوکی
            if (Request.Cookies.ContainsKey(".AspNetCore.Identity.Application"))
            {
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
            }
            if (Request.Cookies.ContainsKey(".AspNetCore.Antiforgery.Zf3VX_qMUzk"))
            {
                Response.Cookies.Delete(".AspNetCore.Antiforgery.Zf3VX_qMUzk");
            }
           // await Controller.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            //NOTE: لازم نیست به جایش هنگام لاگین، بنویس: await _signInManager.SignOutAsync();===>کوکی کاربر قبلی پاک شود--و اگر کاربری لاگین نباشد خطا نمی دهد

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl); // ✅ بهترین روش
            }

        }
        // اگر returnUrl نداشتیم، از Referer استفاده کن
       // var referer = Request.Headers["Referer"].ToString();// i.e.: /Organization/index 
        //var refMini = new Uri(referer).PathAndQuery;
        //if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(refMini))
        //{
            //return LocalRedirect(refMini);
        //}

        return RedirectToAction("IndexNew","Home");
    }
    public async Task<IActionResult> Profile()//(int Id) غلط  ؟   وگرنه مشخصات هر کس را می تواند ببیند
    {
        if (!_userContextService.IsAuthenticated())
            return RedirectToAction("Login");

        var userId = _userContextService.GetCurrentUserId();

        var userDto = await _userService.GetUserByIdAsync(userId.Value);
        CreateUserViewModel userVM = new CreateUserViewModel();
        if (userDto != null)
        {
            userVM.Id = userDto.Id;
            userVM.FirstName = userDto.FirstName;
            userVM.LastName = userDto.LastName;
            userVM.PhoneNumber = userDto.PhoneNumber;
            userVM.Telephone = userDto.Telephone;
            //userVM.CompanyCode = userDto.CompanyID;
            userVM.CompanyId = userDto.CompanyId;
            userVM.CompanyCode = userDto.CompanyCode;
            userVM.CompanyName = userDto.CompanyName;
            userVM.UserName = userDto.UserName;
            userVM.Email = userDto.Email;
            userVM.Description = userDto.Description;
        }
        else
            return NotFound();

        return View(userVM);

    }
    public IActionResult AccessDenied()
    {
        return View();
    }
    public async Task<IActionResult> Navigation()
    {
        List<Menu> list = await _userService.GetAllPermittedMenus();
        ViewBag.Menus = list;
        return PartialView("layout/Navigation");
    }
    //public async Task<PermissionGroup> PermissionGroup()
    //{

    //}

}
