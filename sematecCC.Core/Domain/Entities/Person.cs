using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

[Table("Persons")]
public class Person : AuditingBaseEntity //Owner قدیم _____مالکان کارت
{
    [Display(Name = " نام")]
    [Column(TypeName = "nvarchar(50)")]
    public string? FirstName { get; set; }
    [Display(Name = "  نام خانوادگی  ")]
    [Column(TypeName = "nvarchar(50)")]
    public string? LastName { get; set; }
    public string FullName { get { return $" {FirstName}  {LastName}"; } }

    //[Required(ErrorMessage =" کد ملی خود را وارد نمایید")]
    [Column(TypeName = "varchar(13)")]
    [Display(Name = " کد ملی")]
    public string? NationalCode { get; set; }
    [Required(ErrorMessage = "شماره موبایل خود را وارد نمایید")]
    [Column(TypeName = "varchar(20)")]
    public string Mobile { get; set; }
    [Display(Name = " تلفن    ")]
    [Column(TypeName = "varchar(20)")]
    public string? Phone { get; set; }
    //[Description n]
    [Display(Name = "محل کار  ")]
    [Column(TypeName = "nvarchar(50)")]
    public string? JobPlace { get; set; }
    //modelBuilder.Entity<User>() // replace User with your entity
    //.Property(u => u.Username) // select the property
    //.HasColumnType("varchar(20)"); // set the database type
    [Column(TypeName = "date")]
    [Display(Name = " تاریخ تولد ")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = " تاریخ تولد ")]
    [Column(TypeName = "varchar(10)")]

    public string? BirthDateFa { get; set; }

    public long GivId { get; set; }
    public int CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public virtual Company Company { get; set; }
    public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
}
