namespace SematecCC.Core;

public class PermissiongroupDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupDescription { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public bool IsSelected { get; set; } 

    public PermissiongroupDto() { }


}
