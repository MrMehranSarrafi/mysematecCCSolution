using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

public class Organization: AuditingBaseEntity
{
    [MaxLength(50)]
    public string OrganizationName { get; set; }
    
    [Column(TypeName ="varchar(20)")]
    public string? Telephone { get; set; }
    [Column(TypeName = "varchar(20)")]
    public string? Mobile { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<CardOrder> CardOrders { get; set; } = new HashSet<CardOrder>();
    //public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
}
