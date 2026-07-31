namespace CardNoGenerator.Core;


public class PermissionRequest
{
    public int PermissiongroupId { get; set; }
    public int UserId { get; set; }
    public int[] PermissionList { get; set; }
    public int[] PermissiongroupList { get; set; }
}
