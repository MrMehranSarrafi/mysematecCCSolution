using Core.Enums;
using System.Globalization;

namespace SematecCC.WebUI;

public class CardTransactionVM
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int UserCreated { get; set; }
    public DateTime? DateChanged { get; set; }
    public int? UserChanged { get; set; }

    public int CardId { get; set; }
    public decimal Amount { get; set; }
    public decimal RemainedAmount { get; set; }

    public string Description { get; set; }
    public CardTransactionsStatus Status { get; set; }
     
    public string StatusTitle
    {
        get
        {
            return Status.GetDisplayAttributeValue();
        }
    }

    public string DateCreatedFa
    {
        get
        {

            return DateCreated.ToString("yyyy/MM/dd HH:mm", new CultureInfo("fa-IR"));


        }
    }
    public short Sign { get; set; } = 1;
    //[NotMapped]
    public string SignTitle
    {
        get
        {
            return Sign == 1 ? "افزاینده" : "کاهنده";
        }
    }
    public string CardTransactionTypeTitle { get; set; }
    public string? ProviderId { get; set; }
    public string? BranchId { get; set; }
    public string? TerminalId { get; set; }
}
