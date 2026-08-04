namespace Core.Domain.Entities;
public class PermissiongroupPermission
{
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }

    public int PermissionGroupId { get; set; }
    public Permissiongroup PermissionGroup { get; set; }
}
