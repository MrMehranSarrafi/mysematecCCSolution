using SematecCC.Infra;
using SematecCC.WebUI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;


//using CardNumberGenerator.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Core.Domain.RepositoryContracts;
using Domain.ServiceContracts;
using Domain.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CardsManagementService, CardsManagementService>();
builder.Services.AddScoped<ICardsManagementRepo, CardsManagementRepo>();
builder.Services.AddDbContext<SematecCCDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;//نیاز به تایید ایمیل
    options.Password.RequireDigit = true;//
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<SematecCCDbContext>()
.AddDefaultTokenProviders()//To verify emails and phone numbers and .. 
.AddErrorDescriber<PersianIdentityErrorDescriber>();// ← اضافه کردن پیام‌های فارسی

// ← اضافه کردن تنظیمات امنیتی کوکی
//builder.Services.ConfigureApplicationCookie(options =>
//{
    //options.Cookie.HttpOnly = true; // غیرقابل دسترسی از JS
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;//فقط HTTPS
    //options.Cookie.SameSite = SameSiteMode.Strict;// محافظت در برابر CSRF
    //options.Cookie.Name = "CardsAuthCookie";           // اختیاری: نام کوکی
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(120);// مدت زمان اعتبار کوکی
    //options.SlidingExpiration = true;// اگر کاربر درخواست جدیدی بفرستد، مجددا تایم بالا ریست می شود. 
    //// مسیر صفحه ورود در صورت نیاز به لاگین
    //options.LoginPath = "/UserAccount/Login";

    //// مسیر صفحه عدم دسترسی (Access Denied)
    //options.AccessDeniedPath = "/UserAccount/AccessDenied";
//});
//HttpOnly → کوکی از JavaScript غیرقابل دسترسی است.

//SecurePolicy.Always → فقط روی HTTPS ارسال می‌شود.

//SameSite.Strict → درخواست‌های cross-site (CSRF) محافظت می‌شوند.
builder.Services.AddAuthentication(options =>
{
    // If you primarily use cookies for web app authentication, set this as default.
    // If you need to differentiate between cookie and JWT, you'll manage schemes later.
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // If you want to CHALLENGE using cookies (e.g., redirect to login), also set this.
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
// Configure Cookie Authentication (already done via ConfigureApplicationCookie, but explicit AddCookie is good practice if needed)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    // You can re-apply or override settings here if needed, but ConfigureApplicationCookie usually suffices.
    // Example: options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    // تنظیمات کوکی...
    //options.LoginPath = "/UserAccount/Login";
    options.Cookie.HttpOnly = true; // غیرقابل دسترسی از JS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;//فقط HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;// محافظت در برابر CSRF
    options.Cookie.Name = "CardsAuthCookie";           // اختیاری: نام کوکی
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);// مدت زمان اعتبار کوکی
    options.SlidingExpiration = true;// اگر کاربر درخواست جدیدی بفرستد، مجددا تایم بالا ریست می شود. 
    // مسیر صفحه ورود در صورت نیاز به لاگین
    options.LoginPath = "/UserAccount/Login";

    // مسیر صفحه عدم دسترسی (Access Denied)
    options.AccessDeniedPath = "/UserAccount/AccessDenied";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // تنظیمات JWT...
    // --- THEN Add JWT Bearer Authentication ---
    // This will be used for API calls or specific scenarios that require JWT:
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        // Use configuration directly, ensuring Jwt:Issuer and Jwt:Audience are set in appsettings.json
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // IMPORTANT: Ensure configuration values are not null before accessing .ToString()
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Issuer Key is missing."))) // Changed to IssuerKey for clarity, adjust if your config is different
    };
});


        

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;//JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;


//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"].ToString()))
//    };
//});


builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped</*I*/UserService, UserService>();

builder.Services.AddScoped<IPaymentApiRepo, PaymentApiRepo>();
builder.Services.AddScoped<PaymentApiService, PaymentApiService>();

builder.Services.AddScoped<IOrganizationRepo, OrganizationRepo>();
builder.Services.AddScoped<OrganizationService, OrganizationService>();

builder.Services.AddScoped<ICompanyRepo, CompanyRepo>();
builder.Services.AddScoped<CompanyService, CompanyService>();

builder.Services.AddScoped<IPersonRepo, PersonRepo>();
builder.Services.AddScoped<PersonService, PersonService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddHostedService<TransactionCleanupService>();//Job to clean cardTransactions in their initial state after 10 minutes from their creation time, which is executed every nearly 5 minutes

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



// If you have API controllers that don't use the default routing, you might need this:
app.MapControllers();//ترتیب مهمه . enables attribute routing. But ruins conventional routing
//XOR:
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers(); //  این خط ضروری است برای attribute routing
//});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=IndexNew}/{id?}");


app.Run();
