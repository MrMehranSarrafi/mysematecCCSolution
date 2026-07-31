using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardNoGenerator.Core;
public class Menu    
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    [MaxLength(300)]
    public string Name { get; set; }
    [MaxLength(300)]
    public string Label { get; set; }
    public int? PermissionId { get; set; }
    public  string? Controller { get; set; }
    public  string? Action { get; set; }

    [ForeignKey(nameof(PermissionId))]
    public virtual Permission? Permission { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual Menu Parent {  get; set; }
    public ICollection<Menu> Children { get; set; } = new HashSet<Menu>();
}
