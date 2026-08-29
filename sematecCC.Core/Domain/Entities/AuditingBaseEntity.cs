using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;
public class AuditingBaseEntity
{
    [Column(Order = 1)] // first column
    public int Id { get; set; }

    // ... Other columns ...

    public int UserIdCreated { get; set; }
    [Column(TypeName = "smalldatetime")] //   
    public DateTime DateCreated { get; set; } = DateTime.Now;
   
    public int? UserIdChanged { get; set; }        

    [Column(TypeName = "smalldatetime")]//last column
    public DateTime? DateChanged { get; set; }

}
