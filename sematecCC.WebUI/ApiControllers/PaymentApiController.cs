using CardNoGenerator.Core;
using CardNoGenerator.Core.DTO;
using CardNoGenerator.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardNoGenerator.WebUI.ApiControllers;

[Route("api/Payment")]
[ApiController]
public class PaymentApiController : ControllerBase
{
    private readonly PaymentApiService _paymentApi;
    private readonly IConfiguration _configuration;

    public PaymentApiController(PaymentApiService paymentApi, IConfiguration configuration)
    {
        _paymentApi = paymentApi;
        _configuration = configuration;
    }


    [HttpPost("CardStatus")]
   // [HttpGet("CardStatus")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CardStatusApi([FromBody] CardStatusRequestDto request)
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);
        var cardStatus = await _paymentApi.CardStatusApi(request.CardNumber, roleId, companyId);
        return StatusCode(cardStatus.Response.StatusCode, cardStatus);
    }
    [AcceptVerbs("GET", "POST")]
    [Route("CardCredit")]
    //[HttpPost("CardCredit")]
    //[HttpGet("CardCredit")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public async Task<IActionResult> GetRemainedAmount([FromQuery] string cardNumber) 
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);
        var card = await _paymentApi.GetRemainedAmount(cardNumber, roleId, companyId);
        return StatusCode(card.Response.StatusCode, card);
    }

    [HttpGet("PersonCards")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetPersonCards([FromQuery]  long givId, [FromQuery] string mobileNo)
    {
        //https://localhost:44327/api/payment/PersonCards?givid=2&mobileNo=09199229248
        GetInfoFromUser(out int roleId, out int companyId, out int userId);
        var result = await _paymentApi.GetPersonCards(givId, mobileNo, companyId);
        return StatusCode(result.Response.StatusCode, result);
    }
    

    [HttpPost("token")]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] ApiLoginDto loginInfo, [FromHeader(Name = "Authorization")] string clientHeader)
    {
        /*Test with curl:
         * curl --location 'https://localhost:44327/api/Payment/token' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer MTIzOjEyMw==' \
--data '{
"apiUsername": "Bertino",
"apiPassword": "Bertino"
}'
        --قدیمی:
         * curl -v -X POST "https://localhost:44327/api/PaymentApi/token" ^
-H "Authorization: Basic MTIzOjEyMw==" ^
-H "Content-Type: application/x-www-form-urlencoded" ^
-d "ApiUsername=Giv&ApiPassword=ApiPassword"
        */
        //Basic MTIzOjEyMw==

        //Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0IiwianRpIjoiN2YxOGNmOWItYWI3MS00NTg3LTg4OWUtNzY0NDI3Y2RkMGYwIiwiaWF0IjoxNzc4MTQ0OTExLCJ1bmlxdWVfbmFtZSI6IkJlcnRpbm9BZG1pbiIsInJvbGUiOiIyIiwiQ29tcGFueUlkIjoiMiIsIm5iZiI6MTc3ODE0NDkxMSwiZXhwIjoxNzc4MTUyMTExLCJpc3MiOiJ5b3VyX2lzc3Vlcl9oZXJlIiwiYXVkIjoieW91cl9hdWRpZW5jZV9oZXJlIn0.RpSOi74XYc4850pDCjrz0VFObRmtOLpO1NyH1-Cvd7E
        #region extract data from parameters:
        //client_id:client_secret -->انکد شود  baseپس از ترکیب این دو فیلد با کاراکتر دو نقطه، کل این رشته باید به صورت 64
        //clientId:iuyriwy88, clientSecret:jhs65dfg==>iuyriwy88:jhs65dfg==>base64(iuyriwy88:jhs65dfg)=aXV5cml3eTg4OmpoczY1ZGZn======>Basic aXV5cml3eTg4OmpoczY1ZGZn
        //--header 'Authorization: Basic aXV5cml3eTg4OmpoczY1ZGZn'
        //clientHeader = e.g.: Basic aXV5cml3eTg4OmpoczY1ZGZn
        var result = new ApiResponseDto<LoginApiResponseDto>();
        string base64 = clientHeader.Replace("Basic", "").Replace("Bearer", "").Trim();
        string token = "", refresh_token = "";
        try
        {
            string clientID_Secret = Encoding.UTF8.GetString(Convert.FromBase64String(base64));//client_id:client_secret
            string clientID = clientID_Secret.Split(':')[0];
            string clientSecret = clientID_Secret.Split(':')[1];
            var user = new ApiUserDto();

            if (await _paymentApi.GetApiUser(loginInfo.ApiUsername, loginInfo.ApiPassword, clientID, clientSecret, user) == true)
            {
                //Provide success response:
                (token, refresh_token) = /*_paymentApi.*/await GenerateToken2(user);
                result.Response.Succeeded = true;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Ok;
                result.Response.Message = "Token Generated OK";
                result.Data = new LoginApiResponseDto() { Token = token };
            }
            //else: Provide Failure or NotFound Response
        }
        catch (Exception ex)
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = (int)HttpStatusCodeEnum.InternalServerError;
            result.Response.Error = new Error()
            {
                ErrorMessage = ex.Message
            };
            //return BadRequest(ex.Message);
        }

        #endregion 
        return StatusCode(result.Response.StatusCode, result);
    }

    #region Spend Api and purchase Apis

    //خرج کردن:
    [HttpPost("Spend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]        
    public async Task<IActionResult> Spend([FromBody] SpendApiRequestDto request)
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);
        
        var spend = await _paymentApi.Spend(request, roleId, companyId, userId);
     
        return StatusCode(spend.Response.StatusCode, spend); 
    }
    //تایید خرید:
    [HttpPost("ConfirmSpend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]        
    public async Task<IActionResult> ConfirmSpend( [FromBody] ConfirmOrCancelSpendRequestDto request)
    {         
        GetInfoFromUser(out int roleId, out int companyId, out int userId);           
        var result = await _paymentApi.ConfirmSpend(request.TransactionId, request.ProviderId, roleId, companyId, userId);
        return StatusCode(result.Response.StatusCode, result);
    }
    //لغو خرید:
    [HttpPost("CancelSpend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   
    public async Task<IActionResult> CancelSpend(  [FromBody] ConfirmOrCancelSpendRequestDto request)
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);           
        var result = await _paymentApi.CancelSpend(request.TransactionId, request.ProviderId, roleId, companyId, userId);
        return StatusCode(result.Response.StatusCode, result);
    }
    //لغو همه خریدهای تایم اوت شده:
    [HttpPost("CancelAllTimedoutSpends")]
    public async Task CancelAllTimedoutSpends()
    {
        await _paymentApi.CancelAllTimedoutSpends();
    }
    //استعلام خرید:
    [HttpGet("Inquiry/{transactionId}")] 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Inquiry(int transactionId)
    {
        var cardTransaction = await _paymentApi.GetCardTransaction(transactionId);
        if (cardTransaction == null)
            return NotFound();
        var result = new InquiryApiResponseDto()
        {
            Message = "Ok",
            StatusCode = 200,
            Succeeded = true,
            SpendStatus = cardTransaction.Status.ToString(),
            Amount = cardTransaction.Amount
        };
        return Ok(result);
    }
    #endregion
    //   افزایش اعتبار کارت:
    [HttpPost("CardCreditIncrement")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CardCreditIncrement([FromBody] CardCreditIncrementRequest request)
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);       
        var incrementRequestDto = new SpendApiRequestDto()
        {
            Amount = request.Amount,
            BranchId = request.BranchId,
            CardNumber = request.CardNumber,
            CardPassword = request.Password,
            TerminalId = request.TerminalId,
            ProviderId = request.ProviderId,
            Description = request.Description
        };
        var incrementResult = await _paymentApi.IncrementCardCreditApi(incrementRequestDto, roleId, companyId, userId);

        return StatusCode(incrementResult.Response.StatusCode, incrementResult);
    }
    /// <summary>
    /// SetCardOwner تعیین مالک کارت
    /// </summary>
    [HttpPost("SetCardOwner")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]        
    public async Task<IActionResult> SetCardOwner([FromBody] SetCardOwnerRequest request)
    {
        GetInfoFromUser(out int roleId, out int companyId, out int userId);
        var result = await _paymentApi.SetCardOwner(request, roleId, companyId, userId);
        return StatusCode(result.Response.StatusCode, result);
    }
    #region private
    private void GetInfoFromUser(out int roleId, out int companyId, out int userId)
    {
        //  var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); int.TryParse(userIdClaim?.Value, out userId);
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
        int.TryParse(User.FindFirst(ClaimTypes.Role)?.Value, out roleId);
        int.TryParse(User.FindFirst("CompanyId")?.Value, out companyId);
    }
    private async Task<(string token, string refresh_token)> GenerateToken2(ApiUserDto user)
    {
        // 1. تنظیم زمان انقضا
        var tokenExpireTimeStamp = DateTime.UtcNow.AddHours(2); // حتما از UtcNow استفاده کنید

        // 2. تعریف Claims            
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject: Unique user identifier
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID: Unique token identifier
            new Claim( // Issued At: Token issuance time (Unix Timestamp)
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
            new Claim( // Expiration Time: Token expiration time (Unix Timestamp)
                JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(tokenExpireTimeStamp).ToUnixTimeSeconds().ToString(), // Example: Expires in 1 hour
                ClaimValueTypes.Integer64
            ),
            new Claim(ClaimTypes.Name, user.UserName), // User's name
            // اگر نقش‌ها نام دارند و می‌خواهید رشته ذخیره کنید: // new Claim(ClaimTypes.Role, user.RoleName),
            // اگر نقش‌ها ID هستند و می‌خواهید رشته ذخیره کنید:
            new Claim(ClaimTypes.Role , user.RoleId.ToString()), // User's role ID
            new Claim("CompanyId", user.CompanyId.ToString()) // Custom claim for Company ID
        };

        // 3. کلید امنیتی (بسیار مهم: باید طولانی و تصادفی باشد)
        // نکته: کلید باید در محیط Production حتما از متغیرهای محیطی یا Key Vault خوانده شود، نه Hardcode
        var tokenKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]); //  

        if (tokenKey.Length < 32)
        {
            throw new ArgumentException("JWT Key must be at least 32 bytes long.");
        }

        var securityKey = new SymmetricSecurityKey(tokenKey);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 4. ایجاد توکن با SecurityTokenDescriptor (روش مدرن)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = tokenExpireTimeStamp,
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],   // اگر نیاز به Issuer/Audience دارید اینجا اضافه کنید
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        string token = accessToken, refresh_token = "";
        #region refreshtoken
        // تنظیمات مشابه برای refresh_token
        var refreshTokenExpireTimeStamp = DateTime.UtcNow.AddDays(7);

        var refreshTokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = refreshTokenExpireTimeStamp,
            SigningCredentials = credentials,
        };

        var refreshSecurityToken = tokenHandler.CreateToken(refreshTokenDescriptor);
        var refreshToken = tokenHandler.WriteToken(refreshSecurityToken);
        refresh_token = refreshToken;
        #endregion
        return (token, refresh_token);
    }       
    #endregion
}
