using Application.DTO;
using Application.DTO.Permissions;
using Application.DTO.UserDtos;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.Enums;
using Core.Helpers;
using Core.ServiceContracts;
using System.Transactions;

namespace Core.Services;

public class UserService : MyServicesBase /*: IUserService*/
{
    private readonly IUserRepo _userRepo;
    private readonly IUserContextService _userContext;
    public UserService(IUserRepo userRepo, IUserContextService userContext)
    {
        _userRepo = userRepo;
        _userContext = userContext;
    }

    public async Task<OperationResultDto> CreateUserAsync(EditUserDto user, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        var result = new OperationResultDto();

        if (user.Password?.Trim() != user.ConfirmPassword?.Trim())
        {
            return Fail("رمز های وارد شده، با هم تطابق ندارند.", nameof(user.Password));
        }
        // بررسی تکراری بودن شماره موبایل
        var existingUser = await _userRepo.FindByMobileAsync(user.PhoneNumber, 0);
        if (existingUser != null)
        {
            return Fail("این شماره موبایل وجود دارد.", nameof(user.PhoneNumber));
        }
        //بررسی تکراری بودن یوزرنیم
        existingUser = await _userRepo.FindByUserNameAsync(user.UserName, 0);
        if (existingUser != null)
        {
            return Fail("این یوزرنیم وجود دارد.", nameof(user.UserName));
        }
        // 2. شروع تراکنش
        // از TransactionScope استفاده می‌کنیم تا اگر هر دو عمل موفق نبودند، همه چیز برگشت بخورد

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await _userRepo.CreateUserAsync(user);
                int roleId = isAdmin == true ? 2 : 3; // 2: Admin, 3: User (فرضی)
                await _userRepo.AddRole(user.Id, roleId);
                scope.Complete(); // کامیت تراکنش

                result.Success = true;
                result.Message = "کاربر با موفقیت ایجاد شد.";
                return result;
            }
            catch (Exception ex)
            {
                // اگر خطایی رخ دهد، تراکنش خودکار Rollback می‌شود (چون Complete صدا زده نشد)
                return Fail("در ایجاد کاربر خطا رخ داد: " + ex.Message);
                // log error
            }
        }
    }
    public async Task<List<EditUserDto>> GetAllUsersAsync()
    {
        return await _userRepo.GetAllUsersAsync();
    }
    public async Task<List<EditUserDto>> GetAllUsersAsync(bool? isAdmin, int? companyId, int? currentUserId, string roleName)
    {
        //await _userRepo.Test();
        // Validate inputs
        if (string.IsNullOrEmpty(roleName))
            return new List<EditUserDto>();

        // Admin gets all users
        if (isAdmin == true)
            return await _userRepo.GetAllUsersAsync();

        // Company admin gets all users in their company
        if (roleName == RoleNamesEnum.companyAdmin.ToString() && companyId.HasValue)
            return await _userRepo.GetAllUsersAsync(companyId.Value);

        // Company user gets only themselves
        if (roleName == RoleNamesEnum.companyUser.ToString() && currentUserId.HasValue)
        {
            var user = await _userRepo.GetUserByIdAsync(currentUserId.Value);
            return user != null
                ? new List<EditUserDto> { user }
                : new List<EditUserDto>();
        }

        return new List<EditUserDto>();
    }
    public async Task<OperationResultDto> Delete(int Id)
    {
        var result = new OperationResultDto();
        try
        {
            await _userRepo.Delete(Id);
            result.Success = true;
            result.Message = "کاربر با موفقیت حذف شد";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return result;
    }
    public async Task<OperationResultDto> DisableUser(int Id, int? currentUserId)
    {
        var result = new OperationResultDto();
        if (Id == currentUserId.Value)
        {
            result.Success = false;
            result.Message = "هیچ کاربری امکان غیرفعال کردن خود را ندارد";
            return result;
        }
        try
        {
            await _userRepo.DisableUser(Id);
            await _userRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "کاربر با موفقیت غیرفعال شد";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return result;
    }
    public async Task<OperationResultDto> EnableUser(int Id)
    {
        var result = new OperationResultDto();
        try
        {
            await _userRepo.EnableUser(Id);
            await _userRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "کاربر با موفقیت فعال شد";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<OperationResultDto> Login(LoginDto login)
    {
        var result = new OperationResultDto();
        try
        {
            var user = await _userRepo.FindUser(login.UserName);
            if (user == null)
            {
                result.Success = false;
                result.Message = "کاربر یافت نشد.";
                return result;
            }
            if (user.IsActive == false)
            {
                result.Success = false;
                result.Message = "کاربر غیرفعال است.";
                return result;
            }
            if (await _userRepo.Login(login))
            {
                result.Success = true;
                result.Message = "ورود به سیستم با موفقیت انجام شد.";
            }
            else
            {
                result.Success = false;
                result.Message = "کاربر یافت نشد.";
            }

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = " کاربر یافت نشد. " + "\n" + ex.Message;
        }
        return result;

    }
    public async Task<EditUserDto?> GetUserByIdAsync(int Id)
    {
        var userDto = await _userRepo.GetUserByIdAsync(Id);
        return userDto;
    }

    public async Task<OperationResultDto> UpdateAsync(EditUserDto user)
    {
        var result = new OperationResultDto();
        try
        {
            // بررسی تکراری بودن شماره موبایل
            var existingUser = await _userRepo.FindByMobileAsync(user.PhoneNumber, user.Id);
            if (existingUser != null)
            {
                return OperationResultHelper.Fail("این شماره موبایل وجود دارد ", nameof(user.PhoneNumber));
            }
            //بررسی تکراری بودن یوزرنیم
            existingUser = await _userRepo.FindByUserNameAsync(user.UserName, user.Id);
            if (existingUser != null)
            {
                return OperationResultHelper.Fail(" این نام کاربری وجود دارد ", nameof(user.UserName));

            }

            var userUpdating = await _userRepo.GetUserByIdAsync(user.Id);
            if (userUpdating == null)
            {
                return OperationResultHelper.Fail(" کاربر یافت نشد ");
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                if (user.Password != user.ConfirmPassword)
                {
                    return OperationResultHelper.Fail(" پسورد های وارد شده با هم تطابق ندارند ", nameof(user.Password));
                }
                else
                {
                    await _userRepo.UpdatePasswordAsync(user);

                }
            }


            await _userRepo.UpdateAsync(user);
            result.Success = true;
            result.Message = "کاربر با موفقیت ویرایش شد.";

        }
        catch (Exception ex)
        {
            return OperationResultHelper.Fail("خطا رخ داد \n" + ex.Message);
            //throw;
        }
        return result;
    }

    public async Task<List<EditUserDto>> GetUsersAsync(string usersParams)
    {
        return await _userRepo.GetUsersAsync(usersParams);
    }

    public async Task<OperationResultDto> ChangePassword(ChangePasswordDto passwordInfo)
    {
        var result = new OperationResultDto();
        var user = await _userRepo.FindUserByCredentialsAsync(passwordInfo.UserName, passwordInfo.CurrentPassword);
        if (user == null)
        {
            result.Success = false;
            result.Message = "کاربری با این یوزرنیم یا پسورد وجود ندارد. ";
            result.PropertyName = nameof(passwordInfo.CurrentPassword);
            return result;
        }
        user.Password = passwordInfo.Password;
        user.oldPassword = passwordInfo.CurrentPassword;
        try
        {
            bool updated = await _userRepo.UpdatePasswordAsync(user);
            if (updated)
            {
                result.Success = true;
                result.Message = "پسورد با موفقیت تغییر کرد ";
                return result;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = " در آپدیت رمز کاربر خطا رخ داد. " + "\n" + ex.Message;

            return result;

        }

        return result;

    }

    public async Task<bool> LogOut()
    {
        return await _userRepo.LogOut() && _userContext.IsAuthenticated();
    }

    public async Task<int> GetCompanyId(int? UserId)
    {

        if (UserId.HasValue)
            return await _userRepo.GetCompanyId(UserId);
        return 0;
    }

    public async Task<List<EditUserDto>> GetUsersAsync(string usersParams, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        return await _userRepo.GetUsersAsync(usersParams, isAdmin, currentCompanyId, currentUserId);
    }

    public async Task<List<Menu>> GetAllMenus()
    {
        return await _userRepo.GetAllMenus();
    }
    public async Task<List<Menu>> GetAllPermittedMenus()
    {
        (bool? isAdmin, int? currentUserId) = _userContext.GetAuthenticationIsAdminCurrentUserId();
        if (isAdmin.HasValue && isAdmin.Value == true)
        {
            return await _userRepo.GetAllMenus();
        }
        else
        {
            return await _userRepo.GetAllPermittedMenus(currentUserId);

        }
    }

    public async Task<List<Permissiongroup>> GetPermissionGroups(string roleName, int? currentUserId)
    {
        if (roleName == RoleNamesEnum.admin.ToString())
        {

            return await _userRepo.GetPermissionGroups();
        }
        else if (roleName == RoleNamesEnum.companyAdmin.ToString())
        {
            return await _userRepo.GetPermissionGroups(currentUserId);

        }
        return new List<Permissiongroup>();
    }
    public async Task<List<PermissionDto>> GetUserPermissions(int userId, int? currentUserId, string currentRoleName)
    {
        return await _userRepo.GetUserPermissions(userId, currentUserId, currentRoleName);
    }
    public async Task<OperationResultDto> PermissionGroupCreateAsync(Permissiongroup permissionGroup)
    {
        var result = new OperationResultDto();
        try
        {
            var p = await _userRepo.GetPermissionGroupByNameAsync(permissionGroup.Name.Trim());
            if (p != null)
            {
                return Fail("گروه مجوزها با این نام قبلا تعریف شده است", nameof(Permissiongroup.Name));

            }
            await _userRepo.PermissionGroupCreateAsync(permissionGroup);
            await _userRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات گروه مجوزها با موفقیت ثبت شد.";
        }
        catch (Exception ex)
        {
            return Fail($"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}", "");
        }
        return result;
    }

    public async Task<OperationResultDto> PermissionGroupEdit(Permissiongroup perGroup)
    {
        var result = new OperationResultDto();
        try
        {
            var existingPermissionGroup = await _userRepo.FindPermissionGroupAsync(perGroup.Name.Trim(), perGroup.Id);
            if (existingPermissionGroup != null)
            {
                return OperationResultHelper.Fail("این نام  وجود دارد ", nameof(perGroup.Name));
            }


            var perGroupUpdating = await _userRepo.GetPermissionGroupAsync(perGroup.Id);
            if (perGroup == null)
            {
                return OperationResultHelper.Fail("گروه مجوزها یافت نشد ");
            }




            await _userRepo.PermissionGroupEdit(perGroup);
            await _userRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "گروه مجوزها با موفقیت ویرایش شد.";

        }
        catch (Exception ex)
        {
            return OperationResultHelper.Fail("خطا رخ داد \n" + ex.Message);

        }
        return result;
    }

    public async Task<Permissiongroup> GetPermissionGroupById(int id)
    {

        return await _userRepo.GetPermissionGroupAsync(id);
    }

    public async Task<List<PermissionDto>> GetPermissiongroupPermissions(int permissionGroupId)
    {
        var isAdmin = _userContext.IsUserAdmin();
        var list = await _userRepo.GetPermissiongroupPermissions(permissionGroupId, isAdmin);
        return list;
    }

    public async Task<OperationResultDto> SetPermissiongroupPermissions(int groupId, int[] permissions)
    {
        OperationResultDto result = new OperationResultDto();
        try
        {
            await _userRepo.SetPermissiongroupPermissions(groupId, permissions);
            result.Success = true;
            result.Message = "مجوز های گروه با موفقیت ذخیره شد.";

        }
        catch (Exception ex)
        {

            result.Success = false;
            result.Message = ex.Message + $"\n خطا رخ داد ";
        }
        return result;
    }
    public async Task<OperationResultDto> SetUserPermissions(int userId, int[] permissions)
    {
        OperationResultDto result = new OperationResultDto();
        try
        {
            await _userRepo.SetUserPermissions(userId, permissions);
            result.Success = true;
            result.Message = "مجوز های کاربر با موفقیت ذخیره شد.";

        }
        catch (Exception ex)
        {

            result.Success = false;
            result.Message = ex.Message + $"\n خطا رخ داد ";
        }
        return result;
    }

    public async Task<List<PermissiongroupDto>> GetUserPermissiongroups(int userId, int? currentUserId, string currentRoleName)
    {
        List<PermissiongroupDto> list = new List<PermissiongroupDto>();
        if (currentRoleName == RoleNamesEnum.admin.ToString())
        {
            list = await _userRepo.GetUserAllPermissiongroups(userId);

        }
        else if (currentRoleName == RoleNamesEnum.companyAdmin.ToString())
        {
            list = await _userRepo.GetUserPermissiongroups(userId, currentUserId);//فقط گروه مجوزهایی را که خود ادمین شرکت به آنها دسترسی دارد را می خواند. و به کاربر شرکت خودش دسترسی می دهد یا نمی دهد
        }
        return list;
    }

    public async Task<OperationResultDto> SetUserPermissiongroups(int userId, int[] permissiongroups)
    {
        OperationResultDto result = new OperationResultDto();
        try
        {
            await _userRepo.SetUserPermissiongroups(userId, permissiongroups);
            result.Success = true;
            result.Message = "گروه های مجوزهای کاربر با موفقیت ذخیره شد.";

        }
        catch (Exception ex)
        {

            result.Success = false;
            result.Message = ex.Message + $"\n خطا رخ داد ";
        }
        return result;
    }
}
