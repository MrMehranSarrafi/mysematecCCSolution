namespace CardNoGenerator.Core;

public class CreateUserDto
{
    public  int Id { get; set; }
    public string? FirstName { get; set; }
    public string LastName { get; set; }      
    
    
    public string? CompanyName { get; set; }
    public string CompanyCode { get; set; }
    public int CompanyId { get; set; }


    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string UserName { get; set; }
    //public string NormalizedUserName { get; set; }
    public string? Email { get; set; }
    //public string NormalizedEmail { get; set; }
    //public bool EmailConfirmed { get; set; } = true;
    public string? PhoneNumber { get; set; }
    //public bool PhoneNumberConfirmed { get; set; } = true;
    //public string PasswordHash { get; set; }
    public  string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? oldPassword { get; set; }//وقتی کاربر رمزشو عوض می کنه. البته اگه بعد از لاگین این کار رو بکنه کمتر لازمه

    //public bool TwoFactorEnabled { get; set; } = false;
    //public bool LockoutEnabled { get; set; } = true;
    //public bool AccessFailedCount { get; set; } = false;
    public bool? admin { get; set; }

}
	
	 
	 
	