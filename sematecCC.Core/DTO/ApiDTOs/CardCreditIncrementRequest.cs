namespace SematecCC.Core; 

public class CardCreditIncrementRequest
{
    public string CardNumber { get; set; }
    public string Password { get; set; }
    public decimal Amount { get; set; }
    public string ProviderId { get; set; }
    public string BranchId { get; set; }
    public string TerminalId { get; set; }
    public string Description { get; set; } = "";
}
