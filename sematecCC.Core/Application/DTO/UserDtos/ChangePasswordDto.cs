using System.ComponentModel.DataAnnotations;

namespace Application.DTO.UserDtos;

public class ChangePasswordDto
{
    public int Id { get; set; }        

    [MaxLength(256)]
    [Display(Name = " نام کاربری  ")]
    [Required(ErrorMessage = "نام کاربری ضروری است")]
    public string UserName { get; set; }        
    [Display(Name = " رمز عبور فعلی  ")]
    [Required(ErrorMessage = "وارد کردن رمز عبور فعلی الزامی است")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }
    //[MaxLength (50)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا ۳۲ کاراکتر باشد")]
    [Display(Name = " رمز ")]
    [Required(ErrorMessage ="رمز عبور را وارد نمایید.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    //[MaxLength(50)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا ۳۲ کاراکتر باشد")]
    [Display(Name = " تکرار رمز ")]
    [Compare("Password", ErrorMessage ="رمزها یکسان نمی باشند")]
    [Required(ErrorMessage = "تکرار رمز عبور را وارد نمایید.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

}
