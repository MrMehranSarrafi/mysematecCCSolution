using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Domain.Entities;
public class CardType : AuditingBaseEntity
{
    [StringLength(50)]
    public string Title { get; set; }
    public bool IsChargeable { get; set; } = false;
}
