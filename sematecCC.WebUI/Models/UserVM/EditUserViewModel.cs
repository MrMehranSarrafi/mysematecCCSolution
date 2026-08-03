using System.ComponentModel.DataAnnotations;

namespace SematecCC.WebUI;

public class EditUserViewModel
{
    public int Id { get; set; }
    [MaxLength(50)]
    [Display(Name ="نام")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "نام خانوادگی ضروری است")]
    [MaxLength(50)]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; }

    [MaxLength(50)]
    [Display(Name = "نام شرکت")]
    //[Required(ErrorMessage = "نام شرکت را وارد نمایید")]
    public string? CompanyName { get; set; }

    //[Required(ErrorMessage = "کد شرکت الزامی است")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "کد شرکت باید دقیقاً ۸ رقم عددی باشد")]
    [MaxLength(8)]
    [Display(Name = " کد شرکت  ")]
    public string? CompanyCode { get; set; }//CompanyID  نام قدیم
    [Required(ErrorMessage = "شرکت را انتخاب نمایید")]
    public int CompanyId { get; set; }


    [MaxLength(20)]
    [Display(Name = " تلفن  ")]
    [RegularExpression(@"^[0-9\+\-\(\)\s]*$", ErrorMessage = "فرمت تلفن صحیح نیست")]
    public string? Telephone { get; set; }

    [MaxLength(500)]
    [Display(Name = " توضیحات  ")]
     
    public string? Description { get; set; }

    [Required(ErrorMessage = "نام کاربری ضروری است")]
    [MaxLength(256)]
    [Display(Name = " نام کاربری  ")]
    public string UserName { get; set; }

    //[Required(ErrorMessage = "ایمیل ضروری است")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
    [MaxLength(256)]
    [Display(Name = " ایمیل")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Display(Name = " موبایل  ")]
    [Required(ErrorMessage = "شماره موبایل ضروری است")]
    [MaxLength(20, ErrorMessage = "تعداد کاراکترهای موبایل بیش از حد مجاز است")]
    [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "(فرمت موبایل صحیح نیست  (مثال: 09123456789 ")]
    public string PhoneNumber { get; set; }
     
    //[MaxLength (32)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا ۳۲ کاراکتر باشد")]
    [Display(Name = " رمز ")]
    // NOTE: برای همین است که مدل های ایجاد، با آپدیت یا فقط تغییر رمز عبور متفاوت است.
    // در کنترلر چک  می کنم. می تواند خالی باشد، در آن صورت رمز آپدیت نمی شود.
    //[Required(ErrorMessage = "لطفا رمز را وارد نمایید")]    
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    //[MaxLength(32)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا ۳۲ کاراکتر باشد")]
    [Display(Name = " تکرار رمز ")]
    //[Required(ErrorMessage = "لطفا تکرار رمز را وارد نمایید")]
    //?? هر دو رمز باید یکسان باشد  
    //در کنترلر چک کردم وگرنه CustomValidator
    [Compare("Password",ErrorMessage ="رمز عبور و تکرار رمز عبور یکسان نیستند")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    //public List<SelectListItem> Companies { get; set; } = new List<SelectListItem>();
}
