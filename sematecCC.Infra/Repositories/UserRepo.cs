using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Application.DTO.Permissions;
using Application.DTO.UserDtos;
using Domain.Enums;

namespace SematecCC.Infra;

public class UserRepo : IUserRepo
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SematecCCDbContext _db;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;  // ✅ درست

    public UserRepo(UserManager<ApplicationUser> userManager, SematecCCDbContext db, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _db = db;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public int SaveChanges()
    {
        return _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
    public async Task CreateUserAsync(EditUserDto userDto)//returns the newly-created user's id.
    {
        // مپ کردن DTO به Entity
        var user = new ApplicationUser
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            CompanyId = userDto.CompanyId,
            Telephone = userDto.Telephone,
            Description = userDto.Description,
            UserName = userDto.UserName,
            Email = userDto.Email,
            EmailConfirmed = true, // اگر می‌خوای ایمیل تأیید شده باشه
            PhoneNumber = userDto.PhoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            IsActive = true

        };

        // ایجاد کاربر
        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }

        userDto.Id = user.Id;
        userDto.SecurityStamp = user.SecurityStamp;
        userDto.ConcurrencyStamp = user.ConcurrencyStamp;
    }
    public async Task AddRole(int userId, int roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId + "");
        if (role == null)
        {
            throw new Exception($"نقش مورد نظر با Id={roleId} بافت نشد");
        }
        var user = await _userManager.FindByIdAsync(userId + "");
        var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
        if (!roleResult.Succeeded)
        {
            throw new Exception(string.Join(" | ", roleResult.Errors.Select(e => e.Description)));
        }
    }
    public async Task<bool> UpdateAsync(EditUserDto userDto)
    {
        bool result = true;
        string errorMessage = "";
        var user = await _userManager.FindByIdAsync(userDto.Id + "");//FirstOrDefault(u => u.Id == userDto.Id);

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;

        user.CompanyId = userDto.CompanyId;

        user.Telephone = userDto.Telephone;
        user.Description = userDto.Description;
        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        user.EmailConfirmed = true; // اگر می‌خوای ایمیل تأیید شده باشه
        user.PhoneNumber = userDto.PhoneNumber;
        user.PhoneNumberConfirmed = false;
        user.TwoFactorEnabled = false;
        user.LockoutEnabled = false;
        user.AccessFailedCount = 0;

        var identityResult = await _userManager.UpdateAsync(user);

        if (!identityResult.Succeeded)
        {
            foreach (var err in identityResult.Errors)
            {
                errorMessage += err.Description + "\n";
            }
            throw (new Exception(errorMessage));

        }
        return true;

    }
    public async Task<bool> UpdatePasswordAsync(EditUserDto userDto)
    {
        bool result = true;
        string errorMessage = "";
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

        var identityResult = await _userManager.RemovePasswordAsync(user);
        if (!identityResult.Succeeded)
        {
            foreach (var err in identityResult.Errors)
            {
                errorMessage += err.Description + "\n";
            }
            throw (new Exception(errorMessage));

        }
        var identityResult2 = await _userManager.AddPasswordAsync(user, userDto.Password);
        if (!identityResult2.Succeeded)
        {
            foreach (var err in identityResult2.Errors)
            {
                errorMessage += err.Description + "\n";
            }
            throw (new Exception(errorMessage));

        }
        return true;

    }
    public async Task<List<EditUserDto>> GetAllUsersAsync()
    {
        
        // استفاده از _context.Users به جای _userManager.Users برای کوئری‌های پیشرفته
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.Company)
            .AsQueryable();

        return await (from u in query
                          // اتصال به جدول UserRoles برای دریافت RoleId
                      join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                      from ur in userRoles.DefaultIfEmpty() // Left Join

                          // اتصال به جدول Roles برای دریافت نام نقش (اختیاری)
                      join r in _db.Roles on ur.RoleId equals r.Id into roles
                      from r in roles.DefaultIfEmpty()

                      select new EditUserDto
                      {
                          Id = u.Id,
                          UserName = u.UserName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                          Telephone = u.Telephone,
                          CompanyId = u.CompanyId,
                          CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                          CompanyName = u.Company != null ? u.Company.CompanyName : null,
                          FirstName = u.FirstName,
                          LastName = u.LastName,
                          IsActive = u.IsActive,
                          Role1Id = ur.RoleId,
                          Role1Name = r != null ? r.Name : ""
                      })
                      .OrderBy(u => u.CompanyCode)
                      .ToListAsync();
    }
    public async Task<EditUserDto?> FindUser(string UserName)
    {
        return await _userManager.Users.Where(u => u.UserName == UserName).Select(u => new EditUserDto { Id = u.Id, IsActive = u.IsActive }).FirstOrDefaultAsync();
    }
    public async Task<EditUserDto?> FindByMobileAsync(string phoneNumber, int Id)
    {
        var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Id != Id);

        if (user == null)
            return null;

        return new EditUserDto
        {
            Id = user.Id
        };

    }
    public async Task<EditUserDto?> FindByUserNameAsync(string userName, int Id)
    {
        var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName && u.Id != Id);

        if (user == null)
            return null;

        return new EditUserDto
        {
            Id = user.Id
        };

    }
    public async Task Delete(int Id)
    {
        var user = await _userManager.FindByIdAsync(Id.ToString());
        if (user == null)
            throw new Exception("کاربری وجود ندارد");
        var role = await _roleManager.FindByIdAsync("2");
        await _userManager.RemoveFromRoleAsync(user, role.Name);
        await _userManager.DeleteAsync(user);
    }
    public async Task DisableUser(int Id)
    {
        var user = await _userManager.FindByIdAsync(Id.ToString());
        if (user == null)
            throw new Exception("کاربری وجود ندارد");
        user.IsActive = false;

    }
    public async Task EnableUser(int Id)
    {
        var user = await _userManager.FindByIdAsync(Id.ToString());
        if (user == null)
            throw new Exception("کاربری وجود ندارد");
        user.IsActive = true;

    }
    public async Task<bool> LogOut()
    {
        await _signInManager.SignOutAsync();//یک کوکی برای احراز هویت و کلیم ها داریم که پاک می شود.

        return true;
    }
    public async Task<bool> Login(LoginDto login)
    {
        await _signInManager.SignOutAsync();
        var appUser = await _userManager.FindByNameAsync(login.UserName);
        if (appUser == null)
        {
            //_signInManager.SignInAsync(appUser,  isPersistent: login.RememberMe);     لاگین بدون پسورد
            return false;
        }

        var result = await _signInManager.PasswordSignInAsync(login.UserName, login.PassWord, login.RememberMe, lockoutOnFailure: false);
        //await _signInManager.SignOutAsync(); // Sign out the user

        var role1Id = (_db.UserRoles.Where(ur => ur.UserId == appUser.Id).FirstOrDefault())?.RoleId;

        SortedSet<string> userPermissions = await GetUserPermissionsList(appUser);
        bool UserClaimsAdded = await SetUserClaims(appUser, role1Id.Value, $"{appUser.FirstName} {appUser.LastName}", userPermissions);


        return UserClaimsAdded && result.Succeeded;
    }

    private async Task<SortedSet<string>> GetUserPermissionsList(ApplicationUser appUser)
    {
        var permissions = await _db.UserPermission
            .Where(up => up.UserId == appUser.Id)
            .Select(up => up.Permission.Name)
            .Union(
                _db.UserPermissiongroup
                    .Where(upg => upg.UserId == appUser.Id)
                    .SelectMany(upg => upg.Permissiongroup.PermissiongroupPermissions)
                    .Select(pgp => pgp.Permission.Name)
            )
            .ToListAsync();

        return new SortedSet<string>(permissions);
    }

    private async Task<bool> SetUserClaims(ApplicationUser user, int role1Id, string FullName, SortedSet<string> userPermissions)
    {
        // ۱. بررسی وجود Claim قبلی
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var oldCompanyIdClaim = existingClaims.FirstOrDefault(c => c.Type == "CompanyId");

        // ۲. اگر Claim قبلی وجود داشت، آن را حذف کن
        if (oldCompanyIdClaim != null)
        {
            await _userManager.RemoveClaimAsync(user, oldCompanyIdClaim);
        }

        var companyIdClaim = new Claim("CompanyId", user.CompanyId.ToString());

        // 3. اضافه کردن Claim به کاربر
        var companyResult = await _userManager.AddClaimAsync(user, companyIdClaim);//فقط به دیتابیس رکورد اضافه می کند
        if (!companyResult.Succeeded) return false;

        // ۲. مدیریت Claim نقش اول (Role1Id)
        // بررسی وجود Claim قبلی برای نقش
        var oldRole1IdClaim = existingClaims.FirstOrDefault(c => c.Type == "Role1Id");
        if (oldRole1IdClaim != null)
        {
            await _userManager.RemoveClaimAsync(user, oldRole1IdClaim);// فقط از دیتابیس پاک می‌شود
        }

        var role1IdClaim = new Claim("Role1Id", role1Id.ToString());
        var roleResult = await _userManager.AddClaimAsync(user, role1IdClaim);
        if (!roleResult.Succeeded) return false;


        var oldFullNameClaim = existingClaims.FirstOrDefault(c => c.Type == "FullName");
        if (oldFullNameClaim != null)
        {
            await _userManager.RemoveClaimAsync(user, oldFullNameClaim);
        }
        var FullNameClaim = new Claim("FullName", FullName);
        var fullNameResult = await _userManager.AddClaimAsync(user, FullNameClaim);
        if (!fullNameResult.Succeeded) return false;


        var oldPermissionClaims = existingClaims.Where(c => c.Type == "Permission");
        foreach (var claim in oldPermissionClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        foreach (var permission in userPermissions)
        {
            await _userManager.AddClaimAsync(
                user,
                new Claim("Permission", permission));
        }
        //read from cookie: User.HasClaim("Permission", "User.Create") == True

        await _signInManager.RefreshSignInAsync(user);//به‌روزرسانی کوکی بر اساس دیتابیس
        return true;
    }

    public async Task<EditUserDto?> GetUserByIdAsync(int userId)
    {
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.Company) // برای دسترسی به نام شرکت
            .AsQueryable();

        var result = await (from u in query
                                // لفت جین با جدول UserRoles برای دریافت RoleId
                            join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty() // Left Join

                            where u.Id == userId
                            select new EditUserDto
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Telephone = u.Telephone,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                CompanyId = u.CompanyId,
                                CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                                CompanyName = u.Company != null ? u.Company.CompanyName : null,
                                Description = u.Description,
                                IsActive = u.IsActive,
                                // *** فقط RoleId ***
                                Role1Id = ur.RoleId
                            })
                            .FirstOrDefaultAsync();

        return result;
    }
     
    public async Task<List<EditUserDto>> GetUsersAsync(string usersParams, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {

        if (string.IsNullOrWhiteSpace(usersParams))
        {
            return await GetAllUsersAsync(isAdmin, currentCompanyId, currentUserId);
        }
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.Company) // برای دسترسی به CompanyName و CompanyCode
            .AsQueryable();

        // فیلتر کردن ادمین (اگر نیاز است)
        query = query.Where(u => u.UserName != "admin");

        query = query
       .Where(
            u => u.UserName.Contains(usersParams)
              || u.Company.CompanyName.Contains(usersParams)
              || u.Company.CompanyCode.StartsWith(usersParams)
              || u.PhoneNumber.StartsWith(usersParams)
              || u.LastName.Contains(usersParams)
              || u.Telephone.StartsWith(usersParams)
             ).AsQueryable();

        query = ApplyPermissionsFilter(query, isAdmin, currentCompanyId, currentUserId);

        var result = await (from u in query
                            join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty() // Left Join
                            select new EditUserDto
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Telephone = u.Telephone,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                CompanyId = u.CompanyId,
                                CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                                CompanyName = u.Company != null ? u.Company.CompanyName : null,
                                Description = u.Description,
                                IsActive = u.IsActive,
                                Role1Id = ur.RoleId // دریافت RoleId از طریق Join
                            })
                   .OrderBy(u => u.CompanyCode ?? string.Empty) // مرتب‌سازی امن
                   .ToListAsync();

        return result;
    }
    public async Task<List<EditUserDto>> GetUsersAsync(string usersParams)
    {
        if (string.IsNullOrWhiteSpace(usersParams))
        {
            return await GetAllUsersAsync();
        }
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.Company) // برای دسترسی به CompanyName و CompanyCode
            .AsQueryable();

        // فیلتر کردن ادمین (اگر نیاز است)
        query = query.Where(u => u.UserName != "admin");

        query = query
       .Where(
            u => u.UserName.Contains(usersParams)
              || u.Company.CompanyName.Contains(usersParams)
              || u.Company.CompanyCode.StartsWith(usersParams)
              || u.PhoneNumber.StartsWith(usersParams)
              || u.LastName.Contains(usersParams)
              || u.Telephone.StartsWith(usersParams)
             ).AsQueryable();

        var result = await (from u in query
                            join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty() // Left Join
                            select new EditUserDto
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Telephone = u.Telephone,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                CompanyId = u.CompanyId,
                                CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                                CompanyName = u.Company != null ? u.Company.CompanyName : null,
                                Description = u.Description,
                                IsActive = u.IsActive,
                                Role1Id = ur.RoleId // دریافت RoleId از طریق Join
                            })
                   .OrderBy(u => u.CompanyCode ?? string.Empty) // مرتب‌سازی امن
                   .ToListAsync();

        return result;
    }

    public async Task<EditUserDto> FindUserByCredentialsAsync(string username, string rawPassword)
    {
        // 1. پیدا کردن کاربر با یوزرنیم
        var user = await _userManager.FindByNameAsync(username);

        // یا با ایمیل
        // var user = await _userManager.FindByEmailAsync(username);

        if (user == null)
            return null;

        // 2. بررسی صحت پسورد (PasswordHasher به صورت خودکار هش را چک می‌کند)
        var result = await _userManager.CheckPasswordAsync(user, rawPassword);//پسورد آن کاربر خاص را چک می‌کند! نه معیارهای کلی.


        return result ? new EditUserDto { Id = user.Id, UserName = user.UserName } : null;
    }

    public async Task<List<EditUserDto>> GetAllUsersAsync(bool? isAdmin, int? currentUserCompanyId, int? currentUserId)
    {
        // 1. ساخت کوئری اصلی با Include برای شرکت
        var query = _userManager.Users
            .AsNoTracking()
            .Include(u => u.Company) // برای دسترسی به نام شرکت
            .AsQueryable();

        // 2. اعمال فیلترهای دسترسی (ادمین/شرکت)
        query = ApplyPermissionsFilter(query, isAdmin, currentUserCompanyId, currentUserId);

        // 3. Join با جدول UserRoles برای دریافت RoleId
        // ما از LINQ Query Syntax استفاده می‌کنیم تا بتوانیم به جداول دیگر دسترسی پیدا کنیم
        var result = await (from u in query
                                // لفت جین با جدول UserRoles
                            join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty() // Left Join: اگر کاربری نقش نداشت، RoleId null می‌شود

                            select new EditUserDto
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Telephone = u.Telephone,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                CompanyId = u.CompanyId,
                                CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                                CompanyName = u.Company != null ? u.Company.CompanyName : null,
                                Description = u.Description,
                                IsActive = u.IsActive,

                                // *** اضافه کردن RoleId ***
                                Role1Id = ur.RoleId
                            })
                            .OrderBy(u => u.CompanyId) // مرتب‌سازی
                            .ToListAsync();

        return result;
    }



    private IQueryable<ApplicationUser> ApplyPermissionsFilter(IQueryable<ApplicationUser> query, bool? isAdmin, int? currentUserCompanyId, int? currentUserId)
    {
        if (isAdmin == true)
        {
            // ادمین: همه کاربران را می‌بیند (بدون شرط اضافه)
        }
        else if (currentUserCompanyId.HasValue)
        {
            // کاربر عادی: فقط کاربرانی که CompanyId آنها با کاربر جاری برابر است
            query = query.Where(u => u.CompanyId == currentUserCompanyId);
        }
        else
        {
            // اگر کاربر لاگین نبود یا شرکتش مشخص نبود، لیست خالی برمی‌گردانیم
            query = Enumerable.Empty<ApplicationUser>().AsQueryable();
        }
        return query;
    }

    public async Task<int> GetCompanyId(int? userId)
    {
        if (!userId.HasValue)
            return -1;
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user != null)
            return user.CompanyId;
        else
            return -1;
    }


    public async Task<List<EditUserDto>> GetAllUsersAsync(int currentCompanyId)
    {
         
        return await (from u in _db.Users.AsNoTracking()
                      where u.CompanyId == currentCompanyId
                      join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                      from ur in userRoles.DefaultIfEmpty()
                      select new EditUserDto
                      {
                          Id = u.Id,
                          UserName = u.UserName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                          Telephone = u.Telephone,
                          FirstName = u.FirstName,
                          LastName = u.LastName,
                          CompanyId = u.CompanyId,
                          CompanyCode = u.Company != null ? u.Company.CompanyCode : null,
                          CompanyName = u.Company != null ? u.Company.CompanyName : null,
                          IsActive = u.IsActive,
                          Role1Id = ur.RoleId
                      })
              .OrderBy(u => u.CompanyCode ?? string.Empty)
              .ToListAsync();

    }
    public async Task Test()
    {
        //var pending = await _db.Database.GetPendingMigrationsAsync();

        //if (pending.Any())
        //{
        //    throw new Exception("Database is not up to date.");
        //}
    }
    public async Task<List<Menu>> GetAllMenus()
    {
        //var list = await _db.Menus.AsNoTracking().Where(m=>m.ParentId==null)  .OrderBy(m => m.Id).Include(m=>m.Children).ToListAsync();
        var list = await _db.Menus.Where(m => m.ParentId == null)
        .OrderBy(m => m.Id)
        .Select(m => new Menu
        {
            Id = m.Id,
            Label = m.Label,
            Name = m.Name,
            Action = m.Action,
            Controller = m.Controller,
            PermissionId = m.PermissionId,
            Children = m.Children
                .OrderBy(c => c.Id)
                .ToList()
        })
        .ToListAsync();
        return list;
    }
    #region Permissions
    public async Task<List<Menu>> GetAllPermittedMenus(int? currentUserId)
    {

        var userPermissionIds = await _db.UserPermission
            .Where(p => p.UserId == currentUserId)
            .Select(p => p.PermissionId)
            .Union(
                _db.PermissiongroupPermission
                    .Where(pgp => _db.UserPermissiongroup
                        .Any(ug => ug.UserId == currentUserId && ug.PermissiongroupId == pgp.PermissionGroupId))
                    .Select(pgp => pgp.PermissionId)
            )
            .Distinct()
            .ToListAsync();

        var menus = await _db.Menus
            .Where(m => m.ParentId == null &&
                    
                        (m.PermissionId == null || userPermissionIds.Contains(m.PermissionId.Value))    )
            .OrderBy(m => m.Id)
            .Select(m => new Menu
            {
                Id = m.Id,
                Label = m.Label,
                Name = m.Name,
                Action = m.Action,
                Controller = m.Controller,
                PermissionId = m.PermissionId,
                Children = m.Children
                    .Where(c =>
                     c.Id != 22 &&//گروه مجوزها را برای فقط ادمین باید بیاره نه ادمین شرکت ها
                    ( c.PermissionId == null || userPermissionIds.Contains(c.PermissionId.Value)))
                    .OrderBy(c => c.Id)
                    .ToList()
            })
            .ToListAsync();

        return menus;
    }
    public async Task<List<Permissiongroup>> GetPermissionGroups()
    {
        return await _db.Permissiongroups.ToListAsync();
    }
    public async Task<List<Permissiongroup>> GetPermissionGroups(int? currentUserId)
    {
        if (!currentUserId.HasValue)
            return new List<Permissiongroup>();
        return await _db.Permissiongroups
            .Where(pg => pg.UserPermissiongroups
                .Any(upg => upg.UserId == currentUserId.Value))
            .ToListAsync();
    }

    public async Task<Permissiongroup?> GetPermissionGroupByNameAsync(string name)
    {
        return await _db.Permissiongroups.FirstOrDefaultAsync(pg => pg.Name == name);
    }

    public async Task PermissionGroupCreateAsync(Permissiongroup permissionGroup)
    {
        await _db.Permissiongroups.AddAsync(permissionGroup);
    }

    public async Task<Permissiongroup?> FindPermissionGroupAsync(string Name, int id)
    {
        Name = Name.Trim();
        var permissionGroup = await _db.Permissiongroups.Where(pg => pg.Name == Name && pg.Id != id).FirstOrDefaultAsync();
        return permissionGroup;
    }
    public async Task<Permissiongroup?> GetPermissionGroupAsync(int id)
    {
        return await _db.Permissiongroups.Where(pg => pg.Id == id).FirstOrDefaultAsync();
    }

    public async Task PermissionGroupEdit(Permissiongroup perGroup)
    {
        var permissionGroup = await _db.Permissiongroups.FindAsync(perGroup.Id);
        permissionGroup.Name = perGroup.Name.Trim();
        permissionGroup.Description = perGroup.Description.Trim();
    }

    public async Task<List<PermissionDto>> GetPermissiongroupPermissions(int permissionGroupId, bool? isAdmin)
    {
        //Flat:
        //if (isAdmin.Value == true)
        //{
        //    return await _db.Permissions
        //        .Select(p => new PermissionDto
        //        {
        //            PermissionId = p.Id,
        //            PermissionName = p.Name,
        //            ParentId = p.ParentId,
        //            PermissionLabel = p.Label,

        //            HasPermission = p.PermissiongroupPermissions
        //                .Any(pg => pg.PermissionGroupId == permissionGroupId /*&& pg.PermissionId == p.Id   ؟؟؟؟؟؟؟*/)
        //        })
        //        .ToListAsync();
        //}
        //else if (isAdmin.Value ==false)
        //{
        //    return await _db.Permissions

        //}
        //return new List<PermissionDto>();//NOT Logged in


        //flat:
        if (isAdmin == true)
        {
            // ادمین: همه پرمیژن‌ها + وضعیت تیک خوردن
            return await _db.Permissions
                .AsNoTracking()
                .Select(p => new PermissionDto
                {
                    PermissionId = p.Id,
                    PermissionName = p.Name,
                    ParentId = p.ParentId,
                    PermissionLabel = p.Label,
                    HasPermission = p.PermissiongroupPermissions
                        .Any(pgp => pgp.PermissionGroupId == permissionGroupId)
                })
                .OrderBy(p => p.PermissionId)
                .ToListAsync();
        }

        else if (isAdmin == false)
        {
            // کاربر عادی: فقط پرمیژن‌های تیک خورده
            return await _db.PermissiongroupPermission
                .Where(pgp => pgp.PermissionGroupId == permissionGroupId)
                .AsNoTracking()
                .Select(pgp => new PermissionDto
                {
                    PermissionId = pgp.Permission.Id,
                    PermissionName = pgp.Permission.Name,
                    ParentId = pgp.Permission.ParentId,
                    PermissionLabel = pgp.Permission.Label,
                    HasPermission = true
                })
                .OrderBy(p => p.PermissionId)
                .ToListAsync();
        }

        // لاگین نشده
        return new List<PermissionDto>();


        //    var permissions = await _db.Permissions
        //.Select(p => new PermissionDto
        //{
        //    Id = p.Id,
        //    ParentId = p.ParentId,
        //    Name = p.Name,
        //    Label = p.Label,

        //    HasPermission = p.PermissionGroupPermissions
        //        .Any(x => x.PermissionGroupId == permissionGroupId)
        //})
        //.ToListAsync();

        //    foreach (var parent in permissions)
        //    {
        //        parent.Children = permissions
        //            .Where(x => x.ParentId == parent.Id)
        //            .Cast<PermissionDto>()
        //            .ToList();
        //    }

        //    return permissions
        //        .Where(x => x.ParentId == null)
        //        .ToList();

    }
    public async Task<List<PermissionDto>> GetUserPermissions(int userId, int currentUserId, string currentRoleName)
    {
        //?Has Business Rule : ???
        if (currentRoleName == RoleNamesEnum.admin.ToString())
        {
            //Flat:
            return await _db.Permissions
            .Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                PermissionName = p.Name,
                ParentId = p.ParentId,
                PermissionLabel = p.Label,

                HasPermission = p.UserPermissions
                    .Any(up => up.UserId == userId  /*&& up.PermissionId == p.Id*/)
            })
            .ToListAsync();
        }
        else if (currentRoleName == RoleNamesEnum.companyAdmin.ToString())
        {
            return await _db.Permissions.Where(p => p.UserPermissions.Any(up => up.UserId == currentUserId))
            .Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                PermissionName = p.Name,
                ParentId = p.ParentId,
                PermissionLabel = p.Label,

                HasPermission = p.UserPermissions
                    .Any(up => up.UserId == userId  /*&& up.PermissionId == p.Id*/)
            })
            .ToListAsync();
        }
        return new List<PermissionDto>();

    }
    public async Task<List<PermissionDto>> GetUserPermissions(int userId,int? currentUserId, string currentRoleName)
    {
        if (currentRoleName == RoleNamesEnum.admin.ToString())
        {
            // ادمین: همه پرمیژن‌ها
            //var query = from permission in _db.Permissions
            //            let hasDirect = permission.UserPermissions
            //                .Any(up => up.UserId == userId)

            //            let hasGroup = permission.PermissiongroupPermissions
            //                .Any(pgp => _db.UserPermissiongroup
            //                    .Any(upg =>
            //                        upg.UserId == userId &&
            //                        upg.PermissiongroupId == pgp.PermissionGroupId))

            //            where !hasGroup

            //            select new PermissionDto
            //            {
            //                PermissionId = permission.Id,
            //                PermissionName = permission.Name,
            //                ParentId = permission.ParentId,
            //                PermissionLabel = permission.Label,
            //                HasPermission = hasDirect,
            //                IsReadOnly = false
            //            };

            //return await query.OrderBy(p => p.PermissionId).ToListAsync();
            // همه Permissionها
            var permissions = await _db.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync();

            // مجوزهای مستقیم کاربر
            var directPermissions = await _db.UserPermission
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            var directPermissionSet = directPermissions.ToHashSet();

            // مجوزهای اعطا شده از طریق گروه
            var groupPermissions = await (
                from upg in _db.UserPermissiongroup
                join pgp in _db.PermissiongroupPermission
                    on upg.PermissiongroupId equals pgp.PermissionGroupId
                where upg.UserId == userId
                select pgp.PermissionId
            )
            .Distinct()
            .ToListAsync();

            var groupPermissionSet = groupPermissions.ToHashSet();

            // Permissionهایی که باید نمایش داده شوند
            var visibleIds = permissions
                .Where(p => !groupPermissionSet.Contains(p.Id))
                .Select(p => p.Id)
                .ToHashSet();

            // والدهای آنها را نیز اضافه کن
            var permissionById = permissions.ToDictionary(p => p.Id);

            foreach (var id in visibleIds.ToList())
            {
                int? parentId = permissionById[id].ParentId;

                while (parentId.HasValue)
                {
                    if (!visibleIds.Add(parentId.Value))
                        break;

                    parentId = permissionById[parentId.Value].ParentId;
                }
            }

            return permissions
                .Where(p => visibleIds.Contains(p.Id))
                .Select(p => new PermissionDto
                {
                    PermissionId = p.Id,
                    PermissionName = p.Name,
                    ParentId = p.ParentId,
                    PermissionLabel = p.Label,

                    // فقط مجوزهای مستقیم
                    HasPermission = directPermissionSet.Contains(p.Id),

                    // مجوزهای اعطا شده از طریق گروه فقط نمایش داده می‌شوند
                    IsReadOnly = groupPermissionSet.Contains(p.Id)
                })
                .ToList();
        }
        else if (currentRoleName == RoleNamesEnum.companyAdmin.ToString())
        {
            //// companyAdmin: فقط مجوزهایی که داره
            //var query = from permission in _db.Permissions
            //            let hasDirect = permission.UserPermissions.Any(up => up.UserId == userId)
            //            let hasGroup = permission.PermissiongroupPermissions.Any(pgp =>
            //                _db.UserPermissiongroup.Any(upg =>
            //                    upg.UserId == userId &&
            //                    upg.PermissiongroupId == pgp.PermissionGroupId
            //                )
            //            )
            //            where hasDirect || hasGroup
            //            select new PermissionDto
            //            {
            //                PermissionId = permission.Id,
            //                PermissionName = permission.Name,
            //                ParentId = permission.ParentId,
            //                PermissionLabel = permission.Label,
            //                HasPermission = true,
            //                IsReadOnly = !hasDirect // اگر مستقیم نداشته باشد = ReadOnly
            //            };

            //return await query.OrderBy(p => p.PermissionId).ToListAsync();
            var query =
from permission in _db.Permissions

    // آیا کاربر جاری این مجوز را مستقیماً دارد؟
let currentUserHasDirect =
    permission.UserPermissions.Any(up => up.UserId == currentUserId)

// آیا کاربر جاری این مجوز را از طریق گروه دارد؟
let currentUserHasGroup =
    permission.PermissiongroupPermissions.Any(pgp =>
        _db.UserPermissiongroup.Any(upg =>
            upg.UserId == currentUserId &&
            upg.PermissiongroupId == pgp.PermissionGroupId))

// آیا کاربر هدف این مجوز را مستقیماً دارد؟
let targetUserHasDirect =
    permission.UserPermissions.Any(up => up.UserId == userId)

where currentUserHasDirect || currentUserHasGroup

select new PermissionDto
{
    PermissionId = permission.Id,
    PermissionName = permission.Name,
    ParentId = permission.ParentId,
    PermissionLabel = permission.Label,

    // فقط از جدول UserPermissions کاربر هدف
    HasPermission = targetUserHasDirect,

    // اگر کاربر جاری این مجوز را فقط از طریق گروه دارد
    IsReadOnly = !currentUserHasDirect && currentUserHasGroup
};

            return await query
                .OrderBy(p => p.PermissionId)
                .ToListAsync();
        }

        return new List<PermissionDto>();
    }
    public async Task<List<PermissiongroupDto>> GetUserAllPermissiongroups(int userId)//تمام گروه ها را میاره با قابلیت انتخاب شدن یا نشدن
    {
        return await _db.Permissiongroups.Select(
            pg => new PermissiongroupDto()
            {
                GroupId = pg.Id,
                GroupName = pg.Name,
                GroupDescription = pg.Description,
                IsSelected = pg.UserPermissiongroups.Any(up => up.UserId == userId)


            }
            ).ToListAsync();
    }

    public async Task<List<PermissiongroupDto>> GetUserPermissiongroups(int userId, int? currentUserId)//فقط گروه هایی را که خود کاربر فعل دسترسی دارد 
    {
        return await _db.Permissiongroups.Where(pg => pg.UserPermissiongroups.Any(upg => upg.UserId == currentUserId))
            .Select(
            pg => new PermissiongroupDto()
            {
                GroupId = pg.Id,
                GroupName = pg.Name,
                GroupDescription = pg.Description,
                IsSelected = pg.UserPermissiongroups.Any(up => up.UserId == userId)


            }
            ).ToListAsync();
    }
    public async Task SetPermissiongroupPermissions(int groupId, int[] permissions)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {

            await _db.PermissiongroupPermission
                .Where(p => p.PermissionGroupId == groupId)
                .ExecuteDeleteAsync();

            var newPermissions = permissions.Select(p => new PermissiongroupPermission
            {
                PermissionGroupId = groupId,
                PermissionId = p
            });

            await _db.PermissiongroupPermission.AddRangeAsync(newPermissions);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SetUserPermissions(int userId, int[] permissions)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {

            await _db.UserPermission
                .Where(p => p.UserId == userId)
                .ExecuteDeleteAsync();

            var newPermissions = permissions.Select(p => new UserPermission
            {
                UserId = userId,
                PermissionId = p
            });

            await _db.UserPermission.AddRangeAsync(newPermissions);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SetUserPermissiongroups(int userId, int[] permissiongroupsId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {

            await _db.UserPermissiongroup
                .Where(upg => upg.UserId == userId)
                .ExecuteDeleteAsync();

            var newPermissiongroups = permissiongroupsId.Select(pg => new UserPermissiongroup
            {
                UserId = userId,
                PermissiongroupId = pg
            });

            await _db.UserPermissiongroup.AddRangeAsync(newPermissiongroups);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    #endregion
}
