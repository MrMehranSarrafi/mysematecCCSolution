using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

public class Permission    
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public int? ParentId { get; set; }
    [MaxLength(300)]
    public string Name { get; set; }
    [MaxLength(300)]
    public string Label { get; set; }
    //public ICollection<PermissionGroup> PermissionGroups { get; set; } = new HashSet<PermissionGroup>();
    public ICollection<PermissiongroupPermission> PermissiongroupPermissions { get; set; }
    = new HashSet<PermissiongroupPermission>();
    public ICollection<UserPermission> UserPermissions { get; set; }
   = new HashSet<UserPermission>();
    public ICollection<Permission> Children { get; set; } = new HashSet<Permission>();
    public virtual Permission Parent { get; set; }

}
