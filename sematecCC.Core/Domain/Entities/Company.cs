using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardNoGenerator.Core;

public class Company : AuditingBaseEntity
{
    [Display(Name = " نام شرکت  ")]
    [Required(ErrorMessage = "نام شرکت را وارد نمایید")]
    [MaxLength(50)]
    public string CompanyName { get; set; }

    [Column(TypeName = "varchar(8)")]
    [Required(ErrorMessage = "کد شرکت را وارد نمایید")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "کد شرکت باید دقیقاً ۸ رقم عددی باشد")]
    [Display(Name = " کد شرکت  ")]
    public string CompanyCode { get; set; }
    [NotMapped]
    public string CompanyInfo
    {
        get
        {
            return $"{CompanyCode} - {CompanyName}";
        }
    }
    [MaxLength(50)]
    public string? ApiUsername { get; set; }
    [MaxLength(656)]
    //[DataType(DataType.Password)]
    public string? ApiPassword { get; set; }
    [MaxLength(1024)]//nvarchar(1024) ,but 256 is enough.
    public string? ClientID { get; set; }
    [MaxLength(1024)]
    public string? ClientSecret { get; set; }
    [NotMapped]
    public int RowNo { get; set; }
    public virtual ICollection<CardOrder> CardOrders { get; set; } = new HashSet<CardOrder>();
    public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
}
