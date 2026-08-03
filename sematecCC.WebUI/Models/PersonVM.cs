using SematecCC.Core;
using SematecCC.WebUI.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace SematecCC.WebUI;

public  class PersonVM
{         
    public int Id { get; set; }         
    public DateTime? DateCreated { get; set; }
    public int? UserCreated { get; set; }        
    public DateTime? DateChanged { get; set; }
    public int? UserChanged { get; set; }

    [Display(Name = " نام")]
    //[FirstNameOrLastNameRequired("LastName", " ورود یکی از نام یا نام خانوادگی اجباری است.")]
    public string? FirstName { get; set; }
    //[Required(ErrorMessage ="نام خانوادگی را وارد نمایید.")]
    [FirstNameOrLastNameRequired("FirstName"," ورود یکی از نام خانوادگی یا نام اجباری است.")]
    [Display(Name = "  نام خانوادگی  ")]
    public string? LastName { get; set; }        
    
    //[Required(ErrorMessage =" کد ملی خود را وارد نمایید")]
    [Display(Name = "کد ملی")]
    [RegularExpression(@"^\d{1,10}$", ErrorMessage = "{0} باید فقط شامل عدد و حداکثر 10 رقم باشد")]
    public string? NationalCode { get; set; }

    [Display(Name = " شماره موبایل")]
    [Required(ErrorMessage = "شماره موبایل خود را وارد نمایید")]
    [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "(فرمت موبایل صحیح نیست  (مثال: 09123456789 ")]
    public string Mobile { get; set; }
   
    [Display(Name = " تلفن ")]
    [RegularExpression(@"^\d+(-\d+)?$", ErrorMessage = "{0} فقط می‌تواند شامل عدد و حداکثر یک خط تیره (-) باشد. مثال: 021-33331385 یا 33331385")]
    [MaxLength(20,ErrorMessage =(" تعداد کاراکتر وارد شده بیشتر از حد مجاز است"))]
    public string? Phone { get; set; }

    //[Description n]
    [Display(Name = "محل کار  ")]        
    public string? JobPlace { get; set; }

    [Display(Name = " تاریخ تولد ")]
    public DateTime? BirthDate { get; set; }
    [Display(Name = " تاریخ تولد ")]
    public string?  BirthDateFa { get; set; }
    //{
    //    get
    //    {
    //        if (BirthOfDate == null)
    //            return "";
    //        return BirthOfDate.ToPersian();
    //    }
    //}

    [Display(Name = "کد گیو")]
    [Required(ErrorMessage = " {0} را وارد نمایید.")]
    public long GivId { get; set; }
    [Display(Name = " شرکت  ")]
    [Required(ErrorMessage = " {0} را انتخاب نمایید.")]
    public int CompanyId { get; set; }
    public Company? Company { get; set; }


}
