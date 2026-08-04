using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTO.ApiDTOs;
using Application.DTO.UserDtos;
using Domain.Services;

namespace SematecCC.WebUI.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentApiController1 : ControllerBase
{
    private readonly PaymentApiService _paymentApi;
    private readonly IConfiguration _configuration;
    //private readonly UserService _userService;
    //private readonly HttpContextAccessor _httpContextAccessor;
    //private readonly IUserContextService _userContext;

    public PaymentApiController1(PaymentApiService paymentApi, IConfiguration configuration
        /*, UserService userService, IUserContextService userSContext , HttpContextAccessor accessor, CardsManagementService cardManagementService*/)
    {
        _paymentApi = paymentApi;
        _configuration = configuration;
        // _userService = userService;
        //_userContext = userSContext;
        //_httpContextAccessor = accessor;

    }

    [HttpPost("CardStatusApi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CardStatusApi(string cardNumber, [FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return BadRequest("شماره کارت  نیاز است");
        int roleId, companyId;
        GetInfoFromToken2(token, out roleId, out companyId);
        var cardStatus = await _paymentApi.CardStatusApi(cardNumber, roleId, companyId);
        return Ok(cardStatus);
    }
    [HttpPost("remained-amount")]
    [HttpGet("remained-amount")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]//این یعنی توکن را باید فرستاده باشه
    //همه پارامتر ها [FromBody]
    public async Task<IActionResult> GetRemainedAmount(string cardNumber, string password, [FromHeader(Name = "Authorization")] string token)//در لاگین متفاوته ولی در همه بقیه اکشن ها، توکن ارسال می شود.
    {
        //https://localhost:7083/api/paymentapi/remained-amount?cardnumber=11112222
        //https://localhost:44327/api/paymentapi/remained-amount?cardNumber=1111222200000012
        if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(password))
            return BadRequest("شماره کارت و پسورد نیاز است");
        int roleId, companyId;
        GetInfoFromToken2(token, out roleId, out companyId);
        var card = await _paymentApi.GetRemainedAmount(cardNumber, roleId, companyId);
        return Ok(new
        {
            card.Data
        });

    }
    //[FromHeader(Name = "Authorization")] string token//   اطلاعات امنیتی یا شناسایی سیستمیAuthorization: Bearer <token>
    [HttpPost("token")]
    public async Task<IActionResult> Login([FromForm] ApiLoginDto loginInfo, [FromHeader(Name = "Authorization")] string clientHeader)
    {
        /*Test with curl:
         * curl -v -X POST "https://localhost:44327/api/PaymentApi/token" ^
-H "Authorization: Basic MTIzOjEyMw==" ^
-H "Content-Type: application/x-www-form-urlencoded" ^
-d "ApiUsername=Giv&ApiPassword=ApiPassword"
        */
        #region extract data from parameters:
        //client_id:client_secret -->انکد شود  baseپس از ترکیب این دو فیلد با کاراکتر دو نقطه، کل این رشته باید به صورت 64
        //clientId:iuyriwy88, clientSecret:jhs65dfg==>iuyriwy88:jhs65dfg==>base64(iuyriwy88:jhs65dfg)=aXV5cml3eTg4OmpoczY1ZGZn======>Basic aXV5cml3eTg4OmpoczY1ZGZn
        //--header 'Authorization: Basic aXV5cml3eTg4OmpoczY1ZGZn'
        //clientHeader = e.g.: Basic aXV5cml3eTg4OmpoczY1ZGZn
        string base64 = clientHeader.Replace("Basic", "").Replace("Bearer","").Trim();
        string token="", refresh_token="";
        try
        {
            string clientID_Secret = Encoding.UTF8.GetString(Convert.FromBase64String(base64));//client_id:client_secret
            string clientID = clientID_Secret.Split(':')[0];
            string clientSecret = clientID_Secret.Split(':')[1];
            var user = new ApiUserDto();
            
            if (await _paymentApi.GetApiUser(loginInfo.ApiUsername, loginInfo.ApiPassword, clientID, clientSecret, user) == true)
            {
                //Provide success response:
                (token, refresh_token) = /*_paymentApi.*/await GenerateToken(user);

            }
            //else: Provide Failure or NotFound Response
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        #endregion

        return Ok(token);
    }

    private async Task<(string token, string refresh_token)> GenerateToken(ApiUserDto user)
    {
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
            new Claim(JwtRegisteredClaimNames.Jti,user.Mobile),//JwT unique Id
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),//Issued at
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role , user.RoleId.ToString()),
            new Claim("CompanyId", user.CompanyId.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] + ""));//security key
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
    #region comments
    //Note: خواندن و تایید توکن
    //کد 1 :
    //var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
    //var companyId = claimsPrincipal.FindFirstValue("CompanyId");
    //توضیحات کد:
    //1-tokenHandler.ValidateToken توکن را تایید می کند و یک شیء ClaimsPrincipal را برمی گرداند که شامل ادعاهای توکن است.
    //2- claimsPrincipal.FindFirstValue اولین ادعایی را که نوع آن برابر با "CompanyId" است را پیدا می کن
    //کد2:
    //        var claims = claimsPrincipal.Claims;
    //        var companyIdClaim = claims.FirstOrDefault(c => c.Type == "CompanyId");
    //if (companyIdClaim != null)
    //{
    //    var companyId = companyIdClaim.Value;
    //}
    #endregion
    #region Spend Api and purchase Apis
    //خرج کردن:
    [HttpPost("Spend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Spend([FromHeader(Name = "Authorization")] string token, string cardNumber, string password, decimal amount, string providerId, string branchId, string terminalId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || password.Length != 5)
            return BadRequest("شماره کارت و پسورد را به درستی وارد نمایید.");
        if (!ValidateToken(token))//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] این کار رو میکنه
        {
            return BadRequest("توکن نامعتبر است.");
        }
        int roleId, companyId, userId;
        GetInfoFromToken(token, out roleId, out companyId, out userId);
        var spendRequestDto = new SpendApiRequestDto()
        {
            Amount = amount,
            BranchId = branchId,
            CardNumber = cardNumber,
            CardPassword = password,
            TerminalId = terminalId,
            ProviderId = providerId
        };
        var spend = await _paymentApi.Spend(spendRequestDto, roleId, companyId, userId);

        //return The id of CardTransaction record of mine, as referenceId for him as The response.
        return StatusCode(spend.Response.StatusCode, spend);//Ok(spend);
    }
    #endregion

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
            /*ClaimsPrincipal principal = برای خواندن کلیم ها*/
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
        // token = token.Replace("Basic", "").Trim();//Test?
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
            var roleClaim = principal.FindFirst("RoleId");
            var companyClaim = principal.FindFirst("CompanyId");
            var userIdClaim = principal.FindFirst("UserId");
            roleId = int.Parse(roleClaim?.Value);
            companyId = int.Parse(companyClaim?.Value);
            userId = int.Parse(userIdClaim?.Value);
        }
        catch (Exception ex)
        {
            roleId = -1;
            companyId = -1;
            userId = -1;
        }
    }
    private void GetInfoFromToken2(string token, out int roleId, out int companyId)
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
            token = token.Replace("Basic", "").Replace("Bearer","").Trim();//Test?
            // تبدیل رشته توکن به آبجکت JsonWebToken
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                roleId = -1;
                companyId = -1;
            }

            // دسترسی به کلیم‌ها از طریق ویژگی Claims
            var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "RoleId");
            var companyClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            roleId = int.Parse(roleClaim?.Value);
            companyId = int.Parse(companyClaim?.Value);
        }
        catch (Exception)
        {
            roleId = -1;
            companyId = -1;
        }
    }

}
