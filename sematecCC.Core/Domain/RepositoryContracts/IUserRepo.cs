using Application.DTO.Permissions;
using Application.DTO.UserDtos;
using Core.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Core.Domain.RepositoryContracts;

public interface IUserRepo
{
    //Note: All interface members are implicitly public, so writing 'public' is redundant.
    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task CreateUserAsync(EditUserDto userDto);
    Task AddRole(int userId, int roleId);
    Task<List<EditUserDto>> GetAllUsersAsync();
    Task<List<EditUserDto>> GetAllUsersAsync(int currentCompanyId);
    Task<List<EditUserDto>> GetAllUsersAsync(bool? isAdmin, int? companyId, int? currentUserId);

    Task<EditUserDto?> FindByMobileAsync(string PhoneNumber, int Id);
    Task<EditUserDto?> FindByUserNameAsync(string UserName, int Id);
    Task Delete(int Id);
    Task DisableUser(int Id);
    Task EnableUser(int Id);
    Task<bool> Login(LoginDto login);
    Task<EditUserDto?> GetUserByIdAsync(int Id);
    Task<bool> UpdateAsync(EditUserDto userDto);
    Task<bool> UpdatePasswordAsync(EditUserDto userDto);
    // Task<List<UserDto>> GetUsersAsync(string usersParams, bool? isAdmin);
    Task<EditUserDto> FindUserByCredentialsAsync(string username, string rawPassword);
    Task<EditUserDto?> FindUser(string UserName);
    Task<bool> LogOut();
    Task<int> GetCompanyId(int? userId);
    Task<List<EditUserDto>> GetUsersAsync(string usersParams);
    Task<List<EditUserDto>> GetUsersAsync(string usersParams, bool? isAdmin, int? currentCompanyId, int? currentUserId);
    Task<List<Menu>> GetAllMenus();
    Task<List<Menu>> GetAllPermittedMenus( int? CurrentUserId);
    Task<List<Permissiongroup>> GetPermissionGroups();
    Task<List<Permissiongroup>> GetPermissionGroups(int? currentUserId);
    Task<List<PermissionDto>> GetUserPermissions(int userId, int? currentUserId, string currentRoleName);
    Task<Permissiongroup?> GetPermissionGroupByNameAsync(string name);
    Task PermissionGroupCreateAsync(Permissiongroup permissionGroup);
    Task<Permissiongroup?> FindPermissionGroupAsync(string name, int id);
    Task<Permissiongroup?> GetPermissionGroupAsync(int id);
    Task PermissionGroupEdit(Permissiongroup perGroup);
    Task<List<PermissionDto>> GetPermissiongroupPermissions(int permissionGroupId, bool? isAdmin);
    Task SetPermissiongroupPermissions(int groupId, int[] permissions);
    Task SetUserPermissions(int userId, int[] permissions);
    Task SetUserPermissiongroups(int userId, int[] permissions);
    Task<List<PermissiongroupDto>> GetUserAllPermissiongroups(int userId);
    Task<List<PermissiongroupDto>> GetUserPermissiongroups(int userId, int? currentUserId);
    Task Test();
}
