using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SematecCC.Core;

public class Log
{
    //[Key]   پیش فرض
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  پیش فرض
    public int Id { get; set; }
    [Column(TypeName = "smalldatetime")]
    public DateTime DateDone { get; set; }
    [Required]
    [StringLength(70)]
    public string Operation { get; set; }//e.g تایید سفارش کارت
    [Column(TypeName =("varchar(50)"))]
    public string ObjectName { get; set; }//table name or 
    public int RecordId { get; set; }
    [Required]
    public int UserId { get; set; }
    public byte OperationId { get; set; } 
     
}
