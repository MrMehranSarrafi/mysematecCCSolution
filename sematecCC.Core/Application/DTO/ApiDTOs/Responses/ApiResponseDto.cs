using System.Text.Json.Serialization;

namespace Application.DTO.ApiDTOs.Responses;

public class ApiResponseDto<TDataApiResponse>
{
    public ApiCommonResponseDto Response { get; set; } = new ApiCommonResponseDto();
    public TDataApiResponse Data { get; set; }
}
public class ApiCommonResponseDto
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    // افزودن property برای نگهداری خطا
    public Error Error { get; set; }//=new Error();
}
public record Error
{
    public  byte Code { get; set; }
    public  string ErrorMessage { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CurrentStatus { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]//برای اینکه وقتی null است اصلا نام پراپرتی دیده نشود. وگرنه  CurrentStatus:null
    public string CurrentStatusFa { get; set; }
}


public class ApiDataResponseDto
{

}
public class CancelApiResponseDto:ApiCommonResponseDto
{
    
}

//[Serializable]
public class SpendApiResponseDto//:ApiCommonResponseDto
{
    public decimal Amount { get; set; }
    public int TrackingCode { get; set; }//CardTransaction table Id به عنوان کد پیگیری میفرستم به سیستم مشتری
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CompanyInfo { get; set; }//اگر خرید قبلا انجام شده باشد، مشخصات شرکت که خرید متعلق به ا  است را بر می گرداند، تا مطمین شویم برای همین شرکت است و تکراری است
}
public class InquiryApiResponseDto:ApiCommonResponseDto//ریسپانس استعلام خرید
{
    public string SpendStatus { get; set; }//وضعیت خرید CardTransactionsStatus
    public decimal Amount { get; set; }//مبلغ خرید
}
public class CardStatusApiResponseDto//:ApiCommonResponseDto
{
    public bool IsActive { get; set; }
    public decimal Credit { get; set; }//RemainedAmount
    public string? Owner { get; set; }
    public string? ExpireDateFa { get; set; }
    public bool? IsExpired { get; set; }
}

public class CardCreditApiResponseDto//ریسپانس موجودی 
{
    public bool IsActive { get; set; }
    public bool HasCredit { get; set; }//Credit>0
    public decimal Credit { get; set; }//RemainedAmount
}
public class ConfirmSpendResponseDto// ConfirmSpendApiResponseDto:ApiCommonResponseDto
{
    public int TrackingCode { get; set; }//کد پیگیری = CardTransaction Id
    public string ConfirmDate { get; set; }//Fa(DateChanged) of CardTransaction table
    public string ProviderId { get; set; }//شناسه خرید
}
public class  LoginApiResponseDto
{
    public string Token {  get; set; } 
    
}
public class IncrementCardCreditApiResponseDto 
{
    /// <summary>
    /// تاریخ افزایش اعتبار
    /// </summary>
    public string IncrementDateFa { get; set; }
    /// <summary>
    /// مبلغ افزایش اعتبار
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// موجودی فعلی
    /// </summary>
    public decimal CurrentCredit { get; set; }
    /// <summary>
    /// شماره کارت
    /// </summary>
    public string CardNumber { get; set; }
    /// <summary>
    /// کد پیگیری
    /// </summary>
    public int TrackingCode { get; set; }//CardTransactionId


}

public class CardResponse
{
    public int Id { get; set; }
    public string CardNo { get; set; }
    public string SerialNo { get; set; }
    public decimal Amount { get; set; }
    public decimal RemainedAmount { get; set; }
    public int CardOrderId { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? ExpireDateFa { get; set; }


}
/*
     var response = new ApiResponseDto
     {
         Succeeded = false,
         Message = "", // یا پیغام خطا
         StatusCode = 500,
         Error = new Error
         {
             Code = 1,
             ErrorMessage = "خطای ناشناس"
         }
     };
     return StatusCode(response.StatusCode, response);
*/
/*
  {
  "Succeeded": true,
  "Message": "",
  "StatusCode": 200,
  "Error": null
  }
*/
