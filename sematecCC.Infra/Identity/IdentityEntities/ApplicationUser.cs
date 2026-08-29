using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.IdentityEntities;

public class ApplicationUser : IdentityUser<int>
{
    [MaxLength(50)]
    public string? FirstName { get; set; }
    [MaxLength(50)]
    public string LastName { get; set; }
    
    [Column(TypeName ="varchar(20)")]
    public string? Telephone { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }


    public int CompanyId {  get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public virtual Company Company { get; set; }
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();
    public virtual ICollection<UserPermissiongroup> UserPermissionGroups { get; set; } = new HashSet<UserPermissiongroup>();

}
