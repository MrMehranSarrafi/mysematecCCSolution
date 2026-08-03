using System.ComponentModel.DataAnnotations;

namespace CardNoGenerator.WebUI;

public class OrganizationVM//CreateOrganizationVM
{
    public int Id { get; set; } = 0;
    [Display(Name = "نام سازمان")]
    [Required(ErrorMessage = "نام سازمان الزامی است")]
    [MaxLength(50, ErrorMessage = "نام سازمان نمی‌تواند بیش از ۵۰ کاراکتر باشد")]
    public string OrganizationName { get; set; }

    [Display(Name = "تلفن")]
    [MaxLength(20, ErrorMessage = "تلفن نمی‌تواند بیش از ۲۰ کاراکتر باشد")]
    [RegularExpression(@"^[0-9\+\-\(\)\s]*$", ErrorMessage = "فرمت تلفن صحیح نیست")]
    public string? Telephone { get; set; }

    [Display(Name = "موبایل")]
    [MaxLength(20, ErrorMessage = "تعداد کاراکترهای موبایل بیش از حد مجاز است")]
    [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "(فرمت موبایل صحیح نیست  (مثال: 09123456789 ")]
    
    public string? Mobile { get; set; }

    [Display(Name = "توضیحات")]
    [MaxLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از ۵۰۰ کاراکتر باشد")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "ردیف")]
    public int RowNo { get; set; }
}