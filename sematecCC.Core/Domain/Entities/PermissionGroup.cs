using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;
public class Permissiongroup
{
    public int Id { get; set; }
    
    [MaxLength(300)]
    [Display(Name="نام گروه مجوزها")]
    public string Name { get; set; }
    [MaxLength(300)]
    [Display(Name = "توضیحات")]
    public string Description { get; set; }

    public ICollection<UserPermissiongroup> UserPermissiongroups { get; set; } = new HashSet<UserPermissiongroup>();
    //public ICollection<Permission> Permissions { get; set; }= new HashSet<Permission>();
    public ICollection<PermissiongroupPermission> PermissiongroupPermissions { get; set; }
    = new HashSet<PermissiongroupPermission>();
}
