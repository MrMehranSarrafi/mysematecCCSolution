namespace Application.DTO.ApiDTOs;

public class SpendApiRequestDto
{
   //public string Token{ get; set; }
   public string CardNumber { get; set; }
    public string CardPassword { get; set; }
    public decimal Amount { get; set; }
    public string ProviderId { get; set; }//شناس خرید or ReferenceId
    public string BranchId { get; set; }
    public string TerminalId { get; set; }
    public string? Description { get; set; }
}
//public class CardCreditIncrementApiRequestDto
//{
//    public string CardNumber { get; set; }
//    public string CardPassword { get; set; }
//    public decimal Amount { get; set; }
//    public string ProviderId { get; set; }//شناس خرید or ReferenceId
//    public string BranchId { get; set; }
//    public string TerminalId { get; set; }
//    public string? Description { get; set; }

//}
