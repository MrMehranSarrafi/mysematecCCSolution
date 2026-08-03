namespace SematecCC.Core;

public class PermissiongroupPermissionDto
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }

    public bool HasPermission { get; set; }//if true then SELECTED( ticked) in the tree structure, else NOT

    public List<PermissiongroupPermissionDto> Children { get; set; } = [];
}
