namespace CardNoGenerator.Core;

public class EditUserDto
{
    public  int Id { get; set; }
    public bool? admin { get; set; }
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyCode { get; set; }
    public string? FirstName { get; set; }
    public string LastName { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public  string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? oldPassword { get; set; }//وقتی کاربر رمزشو عوض می کنه. البته اگه بعد از لاگین این کار رو بکنه کمتر لازمه
    //public string NormalizedUserName { get; set; }
    //public string NormalizedEmail { get; set; }
    //public bool EmailConfirmed { get; set; } = true;
    //public bool PhoneNumberConfirmed { get; set; } = true;
    //public string PasswordHash { get; set; }

    //public bool TwoFactorEnabled { get; set; } = false;
    //public bool LockoutEnabled { get; set; } = true;
    //public bool AccessFailedCount { get; set; } = false;
    public bool IsActive { get; set; }
    public string Role1Name{ get; set; }
    public int Role1Id { get; set; }
    public string? SecurityStamp { get; set; }
    public string? ConcurrencyStamp { get; set; }

}
	
	 
	 
	