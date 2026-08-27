using Core.Domain.RepositoryContracts;
using Domain.ServiceContracts;
using Domain.Services;
using Identity.IdentityEntities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.DbContexts;
using System.Text;

namespace SematecCC.Infra;

public static class ConfigureLayer
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CardsManagementService, CardsManagementService>();
        services.AddScoped<ICardsManagementRepo, CardsManagementRepo>();
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped</*I*/UserService, UserService>();

        services.AddScoped<IPaymentApiRepo, PaymentApiRepo>();
        services.AddScoped<PaymentApiService, PaymentApiService>();

        services.AddScoped<IOrganizationRepo, OrganizationRepo>();
        services.AddScoped<OrganizationService, OrganizationService>();

        services.AddScoped<ICompanyRepo, CompanyRepo>();
        services.AddScoped<CompanyService, CompanyService>();

        services.AddScoped<IPersonRepo, PersonRepo>();
        services.AddScoped<PersonService, PersonService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<SematecCCDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
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



        services.AddAuthentication(options =>
        {

            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // If you want to CHALLENGE using cookies (e.g., redirect to login), also set this.
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    // IMPORTANT: Ensure configuration values are not null before accessing .ToString()
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Issuer Key is missing."))) // Changed to IssuerKey for clarity, adjust if your config is different
                };
            });

        

        services.AddHostedService<TransactionCleanupService>();//Job to clean cardTransactions in their initial state after 10 minutes from their creation time, which is executed every nearly 5 minutes


        return services;
    }
}
