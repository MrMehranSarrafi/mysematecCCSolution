using System.ComponentModel.DataAnnotations;

namespace SematecCC.WebUI;

public class LoginViewModel
{
    [Required(ErrorMessage="نام کاربری را وارد نمایید")]
    public string UserName { get; set; }
    [Required(ErrorMessage ="پسورد را وارد نمایید")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }= false;
}
