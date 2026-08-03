using System.ComponentModel.DataAnnotations;

namespace Application.DTO.UserDtos;

public class LoginDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    [DataType(DataType.Password)]
    public string PassWord { get; set; }
    public bool RememberMe { get; set; } = false;
}

