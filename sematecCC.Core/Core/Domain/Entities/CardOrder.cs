using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace Core.Domain.Entities;

public class CardOrder : AuditingBaseEntity
{
    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "مبلغ")]
    //[Range( 0, double.MaxValue, ErrorMessage = "مبلغ باید عددی مثبت باشد")]
    [RegularExpression(@"^\d+(\.\d{1,3})?$", ErrorMessage = "مبلغ باید عددی مثبت و حداکثر ۳ رقم اعشار داشته باشد.")]
    [Required(ErrorMessage = "مبلغ را وارد نمایید")]
    public decimal Amount { get; set; }
    [Display(Name = "تعداد")]
    [Range(0, int.MaxValue, ErrorMessage = "تعداد نمی‌تواند منفی باشد")]
    [Required(ErrorMessage = "تعداد را وارد نمایید")]
    public int Tedad { get; set; }//تعداد

    //tinyint(1,2,3 == اولیه یا جدید،  verified, canceled
    [Display(Name = "وضعیت")]
    public CardOrderStatus Status { get; set; } = CardOrderStatus.NewOrInitial;

    [MaxLength(500)]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();

    public bool IsActive { get; set; } = true;
    [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "مبلغ باید یک عدد صحیح و مثبت باشد.")]
    public int? ExpireDayNumber { get; set; }
    [Column(TypeName = "date")]
    [Display(Name = " تاریخ انقضا ")]
    [DataType(DataType.Date)]
    public DateTime? ExpireDate { get; set; }

    [Display(Name = " تاریخ انقضا ")]
    [Column(TypeName = "varchar(10)")]
    public string? ExpireDateFa { get; set; }

    [NotMapped]
    public bool WillExpire
    {
        get
        {
            if (ExpireDayNumber == null)
                return false;
            return true;
        }
    }
    public int? OrganizationId { get; set; }

    // 🔵 Navigation
    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization? Organization { get; set; }

    [Required(ErrorMessage = "شرکت را انتخاب نمایید")]
    public int CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public virtual Company? Company { get; set; }//مجبور شدم اینو اختیاری تعریف کنم. تا هنگام ایجاد modelstate گیر نده

    [NotMapped]
    public string CompanyCode
    {
        get
        {
            return Company?.CompanyCode ?? "";
        }
    }
    [NotMapped]
    public string CompanyName
    {
        get
        {
            return Company?.CompanyName ?? "";
        }
    }
    [NotMapped]
    public string StatusTitle
    {
        get
        {
            //switch (Status)
            //{
            //    case CardOrderStatus.NewOrInitial:

            //        break;
            //    case CardOrderStatus.Verified:

            //        break;
            //    case CardOrderStatus.Canceled:

            //        break;
            //    default:

            //        break;
            //}
            return Status.GetDisplayAttributeValue();
        }
    }

}
