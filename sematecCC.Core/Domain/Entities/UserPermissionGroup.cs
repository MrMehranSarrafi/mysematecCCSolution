namespace Core.Domain.Entities;
public class UserPermissiongroup
{
     
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissiongroupId { get; set; }

   //[ForeignKey(nameof(PermissiongroupId))] 
    public virtual Permissiongroup Permissiongroup { get; set; }
}
