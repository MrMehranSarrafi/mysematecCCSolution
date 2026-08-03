namespace Application.DTO;

public class OperationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string PropertyName { get; set; } = "";
   // public int StatusCode { get; set; }//Used in web apis returning JsonResult i.e. PaymentApi.
}
