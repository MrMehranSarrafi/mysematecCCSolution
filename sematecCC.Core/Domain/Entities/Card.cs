using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SematecCC.Core;

public class Card: AuditingBaseEntity
{
    [Column(TypeName = "decimal(18,3)")]
    public decimal Amount { get; set; }
    [Column(TypeName = "decimal(18,3)")]
    public decimal RemainedAmount { get; set; }//Balance

    [Column(TypeName = "varchar(7)")]
    public string SerialNo { get; set; }
    //char(16) = CompanyID(8) + SerialNo(7)+  parity(1)
    [Column(TypeName = "varchar(16)")]
    public string CardNo { get; set; }
    //char(5)= numbers and EN characters by random 
    [Column(TypeName = "varchar(5)")]
    public string Password { get; set; }
    //tinyint(1,2,3 == اولیه یا جدید،  verified, canceled
    [Column(TypeName = "tinyint")]
    public CardStatus Status { get; set; }

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "date")]
    [Display(Name = " تاریخ انقضا ")]
    [DataType(DataType.Date)]
    public DateTime? ExpireDate { get; set; }

    [Display(Name = " تاریخ انقضا ")]
    [Column(TypeName = "varchar(10)")]
    public string? ExpireDateFa { get; set; }
   
    [NotMapped]
    public bool IsExpired
    {
        get
        {
            if (ExpireDate == null || ExpireDate.HasValue==false)
                return false;
            if (ExpireDate.Value.Date < DateTime.Now.Date)
                return true;//منقضی شده
             return false;
        }
    }   
    public int CardOrderId { get; set; }
    public int CardTypeId { get; set; }
    //[ForeignKey(nameof(CardTypeId))]
    [ForeignKey(nameof(CardOrderId))]
    public virtual CardOrder CardOrder { get; set; }

    [ForeignKey(nameof(CardTypeId))]
    public virtual CardType CardType { get; set; }

    public int? OrganizationId { get; set; }

    // 🔵 Navigation
    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization? Organization { get; set; }

    public int? OwnerPersonId { get; set; }
    [ForeignKey(nameof(OwnerPersonId))]
    public virtual Person? Owner { get; set; }

    public int CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public virtual Company Company { get; set; }
    public virtual ICollection<CardTransaction> CardTransactions { get; set; } = new HashSet<CardTransaction>();
}
