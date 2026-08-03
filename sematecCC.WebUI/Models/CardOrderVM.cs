using SematecCC.Core;
using System.ComponentModel.DataAnnotations;

namespace SematecCC.WebUI;

public class CardOrderVM
{
    
    public int Id { get; set; }
    [Display(Name = "تاریخ ایجاد")]
     
    public DateTime DateCreated { get; set; }
    
    public int UserIdCreated { get; set; } 
    [Display(Name = "تاریخ ویرایش")]
    
    public DateTime? DateChanged { get; set; }
    [Display(Name = "کاربر ویرایش کننده ")]
    public int? UserIdChanged { get; set; }

     
    [Display(Name ="مبلغ")]
    //[Range( 0, double.MaxValue, ErrorMessage = "مبلغ باید عددی مثبت باشد")]
    [RegularExpression(@"^\d+(\.\d{1,3})?$", ErrorMessage = "مبلغ باید عددی مثبت و حداکثر ۳ رقم اعشار داشته باشد.")]
    [Required (ErrorMessage = "مبلغ را وارد نمایید")]
    public decimal Amount { get; set; }
    [Display(Name = "تعداد")]
    [Range(0, int.MaxValue, ErrorMessage = "تعداد نمی‌تواند منفی باشد")]
    public int Tedad { get; set; }
    
    
    [Required(ErrorMessage = "کد شرکت الزامی است")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "کد شرکت باید دقیقاً ۸ رقم عددی باشد")]
    public string CompanyID { get; set; }
    
     
    [Display(Name = "وضعیت")]
    public CardOrderStatus Status { get; set; }

    public virtual ICollection<Card> Cards { get; set; }= new List<Card>();

}
