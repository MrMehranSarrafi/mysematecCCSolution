namespace Application.DTO;

public class CardDto
{
    public string CardNo { get; set; } //16 chars
    public  string SerialNo { get; set; }//7 chars, and 1 parity or random.

    public string Password { get; set; }//5 chars
    public int RowNo { get; set; }//ردیف 1و2و..تعداد
}
