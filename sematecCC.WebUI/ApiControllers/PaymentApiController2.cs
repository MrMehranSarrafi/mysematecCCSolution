using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Enums;
using Core.Services;
using Application.DTO;
using Application.DTO.ApiDTOs;
using Application.DTO.UserDtos;
using Application.DTO.ApiDTOs.Responses;

namespace SematecCC.WebUI.ApiControllers;

[Route("api/Payment2")]
[ApiController]
public class PaymentApiController2 : ControllerBase
{
    private readonly PaymentApiService _paymentApi;
    private readonly IConfiguration _configuration;

    public PaymentApiController2(PaymentApiService paymentApi, IConfiguration configuration)
    {
        _paymentApi = paymentApi;
        _configuration = configuration;
    }

    [HttpPost("CardStatus")]
    [HttpGet("CardStatus")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CardStatusApi([FromBody] CardStatusRequestDto request/*, [FromHeader(Name = "Authorization")] string token*/)
    {
        //[FromHeader(Name = "Authorization")] string token//   اطلاعات امنیتی یا شناسایی سیستمیAuthorization: Bearer <token>
        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
        //GetInfoFromToken(token, out roleId, out companyId, out userId);
        var cardStatus = await _paymentApi.CardStatusApi(request.CardNumber, roleId, companyId);
        return StatusCode(cardStatus.Response.StatusCode, cardStatus);
    }
    [HttpPost("CardCredit")]
    [HttpGet("CardCredit")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]//این یعنی توکن را باید فرستاده باشه
    public async Task<IActionResult> GetRemainedAmount([FromBody] string cardNumber/*, [FromHeader(Name = "Authorization")] string token*/)//در لاگین متفاوته ولی در همه بقیه اکشن ها، توکن ارسال می شود.
    {
        /*
         دو سناریو داریم:

-1-🔹 حالت Web + Cookie Auth:User از Cookie میاد
-2-حالت API + JWT : User از JWT داخل Header ساخته میشه.
        در هر دو حالت: User  یکی است - 
        فقط Authentication Scheme فرق می‌کنه
        پس:
        توکن دستی parse نکن
        از User.Claims استفاده کن
        اصلاً Authorization header رو پارامتر متد نگیر

         */

        //https://localhost:44327/api/paymentapi/remained-amount?cardNumber=1111222200000012

        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken(token, out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);


        var card = await _paymentApi.GetRemainedAmount(cardNumber, roleId, companyId);
        return Ok(card);

    }

    private void GetInfoFromUser(out int roleId, out int companyId, out int userId)
    {

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        int.TryParse(userIdClaim?.Value, out  userId);//نکته: userId is defined just here.

        // برای خواندن RoleId
        var roleIdClaim = User.FindFirst(ClaimTypes.Role);
        int.TryParse(roleIdClaim?.Value, out  roleId);
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        roleId = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
        int.TryParse(User.FindFirst("CompanyId")?.Value, out companyId);
    }

    [HttpPost("token")]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] ApiLoginDto loginInfo, [FromHeader(Name = "Authorization")] string clientHeader)
    {
        /*Test with curl:
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

        //return Ok(token);
        return StatusCode(result.Response.StatusCode, result);
    }

    #region Spend Api and purchase Apis

    //خرج کردن:
    [HttpPost("Spend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //public async Task<IActionResult> Spend([FromHeader(Name = "Authorization")] string token,
    //    [FromForm] string cardNumber, [FromForm] string password, [FromForm] decimal amount,
    //    [FromForm] string providerId, [FromForm] string branchId/* کد شعبه/کد فروشگاه */
    //    , [FromForm] string terminalId/*شناسه صندوق فروشگاه*/, [FromForm] string description = "")//transactionId(حالت کلی)(purchaseId  شناسه خرید)(or ProviderId مشتری توسط اپلیکیشن کد بارکد ایجاد میکنه و منحصر به خودشه؛ هر دو را  داره من یکی کرده ام)
    public async Task<IActionResult> Spend([FromBody] SpendApiRequestDto request/*, [FromHeader(Name = "Authorization")] string token*/)
    {
        //?cardNumber=1111000100000035&password=Whu41&amount=1000000&providerId=1234&branchId=11&terminalId=2

        //if (!ValidateToken(token))//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] این کار رو میکنه
        //{
        //return UnprocessableEntity("توکن نامعتبر است.");//? فقط تست کرده و کامنت کن
        //}
        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
        //var spendRequestDto = new SpendApiRequestDto()
        //{
        //    Amount = amount,
        //    BranchId = branchId,
        //    CardNumber = cardNumber,
        //    CardPassword = password,
        //    TerminalId = terminalId,
        //    ProviderId = providerId,//که مشتری به من میده؛ شناسه خرید
        //    Description = description
        //};
        var spend = await _paymentApi.Spend(request, roleId, companyId, userId);

        //return The id of CardTransaction record of mine, as referenceId for him as The response.
        return StatusCode(spend.Response.StatusCode, spend);//Ok(spend);
    }
    //تایید خرید:
    [HttpPost("ConfirmSpend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //public async Task<IActionResult> ConfirmSpend([FromHeader(Name = "Authorization")] string token, [FromForm] int transactionId, [FromForm] string providerId)/*transactionId  is the id of CardTransaction table--- providerId= شناسه خرید */
    public async Task<IActionResult> ConfirmSpend(/*[FromHeader(Name = "Authorization")] string token,*/ [FromBody] ConfirmOrCancelSpendRequestDto request)
    {

        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
        var result = await _paymentApi.ConfirmSpend(request.TransactionId, request.ProviderId, roleId, companyId, userId);
        return StatusCode(result.Response.StatusCode, result);
    }
    //لغو خرید:
    [HttpPost("CancelSpend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //public async Task<IActionResult> CancelSpend([FromHeader(Name = "Authorization")] string token, [FromForm] int transactionId, [FromForm] string providerId)//شماره مرجع و شناسه خرید  
    public async Task<IActionResult> CancelSpend(/*[FromHeader(Name = "Authorization")] string token,*/ [FromBody] ConfirmOrCancelSpendRequestDto request)
    {
        int roleId, companyId; int userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
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
    [HttpGet("Inquiry/{transactionId}")]// از طریق شناسه خرید خودش نه. CardTransaction Id of my table
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Inquiry(int transactionId)
    {
        //Tracking Code کد پیگیری که من به او داده ام
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
    //public async Task<IActionResult> CardCreditIncrement([FromHeader(Name = "Authorization")] string token,
    //    [FromForm] string cardNumber, [FromForm] string password, [FromForm] decimal amount,
    //    [FromForm] string providerId, [FromForm] string branchId, [FromForm] string terminalId, [FromForm] string description = "")
    public async Task<IActionResult> CardCreditIncrement(/*[FromHeader(Name = "Authorization")] string token,*/
        [FromBody] CardCreditIncrementRequest request)

    {
        //branchId: کد شعبه/کد فروشگاه **//terminalId: شناسه صندوق فروشگاه*/
        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
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
    //public async Task<IActionResult> SetCardOwner([FromHeader(Name = "Authorization")] string token, [FromForm] string cardNumber, [FromForm] string password, PersonRequestDto personRequest)
    //
    //public async Task<IActionResult> SetCardOwner([FromHeader(Name = "Authorization")] string token,
    //        [FromBody] string cardNumber,
    //        [FromBody] string password,
    //        [FromBody] PersonRequestDto personRequest)//نکته: We can have multiple [FromBody] or multiple [FromForm]; but NOT both of them at the same time, in a mixed way.
    //                                                  //([FromHeader(Name = "Authorization")] string token, [FromForm] SetCardOwnerRequest request)//نکته: 1 آبجکت بهتره    
    //MUST BE 1 OBject
    public async Task<IActionResult> SetCardOwner(/*[FromHeader(Name = "Authorization")] string token, */[FromBody] SetCardOwnerRequest request)
    {

        int roleId, companyId, userId;
        GetInfoFromUser(out roleId, out companyId, out userId);
        //GetInfoFromToken2(token, out roleId, out companyId, out userId);
        var result = await _paymentApi.SetCardOwner(request, roleId, companyId, userId);
        return StatusCode(result.Response.StatusCode, result);
    }
    #region private
    private async Task<(string token, string refresh_token)> GenerateToken(ApiUserDto user)
    {
        //روش قدیمی:
        var claims1 = new[]//تنها نکته ای که باید توجه داشته باشید این است که باید از نام های Claim های سفارشی به طور یکنواخت در سیستم خود استفاده کنید.
       {//همه جا از همین نام ها استفاده کنی.//می تونی اینام بسازی MyClaimsEnum
            new Claim("UserId", user.UserId.ToString()),//Subject
            new Claim("Mobile",user.Mobile),//JwT unique Id
            new Claim("IssuedAt", DateTime.UtcNow.ToString()),//Issued at
            new Claim("Username", user.UserName),
            new Claim("RoleId" , user.RoleId.ToString()),
            new Claim("CompanyId", user.CompanyId.ToString())
        };
        string token = "", refresh_token = "";
        #region main
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),//Subject
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),//JwT unique Id
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),//Issued at
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role , user.RoleId.ToString()),
            new Claim("CompanyId", user.CompanyId.ToString())

        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));//security key
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenGenerator = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
            claims1,
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials: creds);
        var tokenHandler = new JwtSecurityTokenHandler();
        token = tokenHandler.WriteToken(tokenGenerator);//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIxIiwiTW9iaWxlIjoiMDkxMjEzMDc4MTYiLCJJc3N1ZWRBdCI6IjUvNC8yMDI2IDI6MjA6NDkgUE0iLCJVc2VybmFtZSI6ImFkbWluIiwiUm9sZUlkIjoiMSIsIkNvbXBhbnlJZCI6IjEiLCJleHAiOjE3Nzc5MTE2NzMsImlzcyI6InlvdXJfaXNzdWVyX2hlcmUiLCJhdWQiOiJ5b3VyX2F1ZGllbmNlX2hlcmUifQ.F4dX7snJVb_gS6y2xJphYRftnaFsj7DIX94KJ_dmy0Y

        #endregion
        return (token, refresh_token);
    }
    private async Task<(string token, string refresh_token)> GenerateToken2(ApiUserDto user)
    {
        // 1. تنظیم زمان انقضا
        var tokenExpireTimeStamp = DateTime.UtcNow.AddHours(2); // حتما از UtcNow استفاده کنید

        // 2. تعریف Claims
        var claims0 = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject باید یکتا باشد (مثلا UserId)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID منحصر به فرد توکن
                //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),//Issued at
                new Claim(
                        JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64 // نوع داده را هم مشخص کن
                    ),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role , user.RoleId.ToString(), ClaimValueTypes.Integer64),
                new Claim("CompanyId", user.CompanyId.ToString()) // اضافه کردن کلیم‌های سفارشی
                // اگر نقش دارید: new Claim(ClaimTypes.Role, roleName)
            };
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
            // اگر نقش‌ها نام دارند و می‌خواهید رشته ذخیره کنید:
            // new Claim(ClaimTypes.Role, user.RoleName),
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
    private bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"].ToString());
        var validationParameters = new TokenValidationParameters//Must Exactly be the same as settings in program.cs:
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,// اطمینان از اینکه توکن منقضی نشده است
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
            //,ClockSkew = TimeSpan.Zero // اگر نیاز به تطابق دقیق زمان دارید
        };
        try
        {
            //برای خواندن کلیم ها*/
            /*ClaimsPrincipal principal = */
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            //var roleId = principal?.FindFirst("RoleId")?.Value;
            return true;


        }
        catch (SecurityTokenExpiredException ex)
        {
            // اگر می‌خواهید نوع خطا را تشخیص دهید
            return false;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            // امضای توکن نامعتبر است
            return false;
        }
        catch (Exception ex)
        {
            // سایر خطاها
            return false;
        }
    }
    private void GetInfoFromToken(string token, out int roleId, out int companyId, out int userId)
    {
        token = token.Replace("Basic", "").Replace("Bearer", "").Trim();//Test?
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"].ToString());
        var validationParameters = new TokenValidationParameters//Must Exactly be the same as settings in program.cs:
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };
        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            /*نکته مهم
            در JWT وقتی توکن validate می‌شود،
            بعضی از claimهای استاندارد مثل  sub به ClaimTypes.NameIdentifier مپ (map) می‌شوند.
            
            یعنی JwtRegisteredClaimNames.Sub  // "sub"  ===> maps to ==>ClaimTypes.NameIdentifier
// "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"


             */
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);//(JwtRegisteredClaimNames.Sub);
            var companyClaim = principal.FindFirst("CompanyId");
            var roleClaim = principal.FindFirst(ClaimTypes.Role);//FindFirst("role"); new Claim(ClaimTypes.Role , user.RoleId.ToString()),
            userId = int.Parse(userIdClaim?.Value);
            var userId2 = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            roleId = int.Parse(roleClaim?.Value);
            companyId = int.Parse(companyClaim?.Value);

        }
        catch (Exception ex)
        {
            roleId = -1;
            companyId = -1;
            userId = -1;
        }
    }
    private void GetInfoFromToken2(string token, out int roleId, out int companyId, out int userId)
    {
        try
        {
            /*
             var authHeader = Request.Headers["Authorization"].ToString();
             if (authHeader.StartsWith("Bearer "))
             {
                  token = authHeader.Substring(7); // حذف "Bearer "
             }
             else
             {
                 return Unauthorized("توکن معتبر یافت نشد");
             }
            */
            token = token.Replace("Basic", "").Replace("Bearer", "").Trim();//Test?
            // تبدیل رشته توکن به آبجکت JsonWebToken
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                roleId = -1;
                companyId = -1;
            }

            // دسترسی به کلیم‌ها از طریق ویژگی Claims
            var userClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub");
            var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role");
            var companyClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            userId = int.Parse(userClaim?.Value);
            roleId = int.Parse(roleClaim?.Value);
            companyId = int.Parse(companyClaim?.Value);
        }
        catch (Exception ex)
        {
            roleId = -1;
            companyId = -1;
            userId = -1;
        }
    }
    #endregion
}
