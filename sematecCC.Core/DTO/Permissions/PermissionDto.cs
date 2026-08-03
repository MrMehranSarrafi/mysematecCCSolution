namespace SematecCC.Core;

public class PermissionDto
{
    public int PermissionId { get; set; }
    public int? ParentId { get; set; }
    public string PermissionName { get; set; }
    public string PermissionLabel { get; set; }
    public bool HasPermission { get; set; }////Selected, if user has that permission directly(from UserPermission table) or via Permissiongroup(from PermissiongroupPermission table)
    public bool IsReadOnly { get; set; }
    public PermissionDto() { }


}
