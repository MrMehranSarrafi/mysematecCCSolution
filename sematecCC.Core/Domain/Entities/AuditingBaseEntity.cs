using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;
public class AuditingBaseEntity
{
    [Column(Order = 1)] // اول
    public int Id { get; set; }

    // ... سایر فیلدهای اصلی کلاس پایه ...

    public int UserIdCreated { get; set; }
    [Column(TypeName = "smalldatetime")] // نزدیک به انتها
    public DateTime DateCreated { get; set; } = DateTime.Now;
   
    public int? UserIdChanged { get; set; }        

    [Column(TypeName = "smalldatetime")]
    public DateTime? DateChanged { get; set; }

}
