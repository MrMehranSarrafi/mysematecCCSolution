namespace SematecCC.Core;

public class PersonRequestDto
{
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }
        public string Mobile { get; set; }
        public string? Phone { get; set; }
        public string? JobPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthDateFa { get; set; }

        public long GivId { get; set; }
        public int CompanyId { get; set; }
}

//OR:
public class SetCardOwnerRequest//Dto
{
    public string CardNumber { get; set; }
    public string Password { get; set; }
    public PersonRequestDto Person { get; set; }
}
