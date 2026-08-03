using System.ComponentModel.DataAnnotations;

namespace CardNoGenerator.WebUI;

public class CardCreditVM
{
    public int Id { get; set; }
    [Required]
    public int CardId { get; set; }
    [Display(Name = "مبلغ")]
    [RegularExpression(@"^\d+(\.\d{1,3})?$", ErrorMessage = "مبلغ باید عددی مثبت و حداکثر ۳ رقم اعشار داشته باشد.")]
    [Required(ErrorMessage = "مبلغ را وارد نمایید")]
    public  decimal Amount { get; set; }
    [Display(Name ="توضیحات")]
    [MaxLength(4000,ErrorMessage = "{0} حداکثر می تواند دارای 4000 کاراکتر باشد")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

}
