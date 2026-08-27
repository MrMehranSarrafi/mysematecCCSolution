using Core.Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

public class CardOrder : AuditingBaseEntity
{
    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "مبلغ")]
    [RegularExpression(@"^\d+(\.\d{1,3})?$",
        ErrorMessage = "مبلغ باید عددی مثبت و حداکثر ۳ رقم اعشار داشته باشد.")]
    [Required(ErrorMessage = "مبلغ را وارد نمایید")]
    public decimal Amount { get; set; }

    [Display(Name = "تعداد")]
    [Range(0, int.MaxValue, ErrorMessage = "تعداد نمی‌تواند منفی باشد")]
    [Required(ErrorMessage = "تعداد را وارد نمایید")]
    public int Tedad { get; set; }

    [Display(Name = "وضعیت")]
    public CardOrderStatus Status { get; set; } = CardOrderStatus.NewOrInitial;

    [MaxLength(500)]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();

    public bool IsActive { get; set; } = true;

    [RegularExpression(@"^[1-9]\d*$",
        ErrorMessage = "مبلغ باید یک عدد صحیح و مثبت باشد.")]
    public int? ExpireDayNumber { get; set; }

    [Column(TypeName = "date")]
    [Display(Name = "تاریخ انقضا")]
    [DataType(DataType.Date)]
    public DateTime? ExpireDate { get; set; }

    [Column(TypeName = "varchar(10)")]
    [Display(Name = "تاریخ انقضا")]
    public string? ExpireDateFa { get; set; }

    [NotMapped]
    public bool WillExpire => ExpireDayNumber != null;

    public int? OrganizationId { get; set; }

    // Navigation
    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization? Organization { get; set; }

    [Required(ErrorMessage = "شرکت را انتخاب نمایید")]
    public int CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public virtual Company? Company { get; set; }

    [NotMapped]
    public string CompanyCode => Company?.CompanyCode ?? string.Empty;

    [NotMapped]
    public string CompanyName => Company?.CompanyName ?? string.Empty;

    [NotMapped]
    public string StatusTitle => Status.GetDisplayAttributeValue();
}