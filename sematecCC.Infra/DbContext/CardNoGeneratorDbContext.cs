using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Enums;

namespace SematecCC.Infra;

public class SematecCCDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>//DbContext
{
    //IdentityUser
    public SematecCCDbContext(DbContextOptions<SematecCCDbContext> options) : base(options)
    {

    }
    //public DbSet<Customer> Customers { get; set; }
    //public DbSet<ApplicationUser> IdentityUsers { get; set; }//پیش فرض داره
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardOrder> CardOrders { get; set; }
    public DbSet<CardTransaction> CardTransactions { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<CardTransactionType> CardTransactionTypes { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<CardType> CardTypes { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermission { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Permissiongroup> Permissiongroups { get; set; }
    public DbSet<UserPermissiongroup> UserPermissiongroup { get; set; }
    public DbSet<PermissiongroupPermission> PermissiongroupPermission { get;set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // ========== پیکربندی خودکار برای تمام کلاس‌های ارث‌برنده از AuditingBaseEntity ==========
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditingBaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);

                // تنظیم مقدار پیش‌فرض GETDATE() برای DateCreated
                entity.Property(nameof(AuditingBaseEntity.DateCreated))
                      .HasDefaultValueSql("GETDATE()")
                      .ValueGeneratedOnAdd();

                // تنظیمات اختیاری: به روزرسانی خودکار DateChanged
                //entity.Property(nameof(AuditingBaseEntity.DateChanged))
                //.ValueGeneratedOnUpdate();
            }
        }
        // ========== پایان پیکربندی خودکار ==========

        modelBuilder.Entity<Company>().HasData(new Company { Id = 1, CompanyCode = "01307816", CompanyName = "شرکت گیو", UserIdCreated = 1, DateCreated = DateTime.Now });

        modelBuilder.Entity<Company>(entity =>
        {
            // Company -> CardOrders (یک به چند)
            entity.HasMany(c => c.CardOrders)
                .WithOne(co => co.Company)
                .HasForeignKey(co => co.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔴 Restrict: جلوگیری از حذف شرکت با سفارش

            // Company -> Cards (یک به چند)
            entity.HasMany(c => c.Cards)
                .WithOne(ca => ca.Company)
                .HasForeignKey(ca => ca.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔴 Restrict: جلوگیری از حذف شرکت با کارت

            // ایندکس یکتا برای CompanyName
            entity.HasIndex(e => e.CompanyName)
                  .IsUnique()
                  .HasDatabaseName("IX_Companies_CompanyName_Unique");

            // ایندکس یکتا برای CompanyCode
            entity.HasIndex(e => e.CompanyCode)
                  .IsUnique()
                  .HasDatabaseName("IX_Companies_CompanyCode_Unique");

        });

        modelBuilder.Entity<CardOrder>(entity =>
        {
            // CardOrder -> Company (چند به یک)
            entity.HasOne(co => co.Company)
                .WithMany(c => c.CardOrders)
                .HasForeignKey(co => co.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔴 Restrict (تکرار برای وضوح)

            entity.HasOne(co => co.Organization)
                .WithMany(o => o.CardOrders)
                .HasForeignKey(co => co.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);  // 🟡 SetNull: اگر سازمان حذف شد، null شود

            entity.Property(c => c.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(c => c.IsActive).HasDefaultValue(+1);
            entity.HasOne(c => c.Company)
                .WithMany(cmp => cmp.Cards)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔴 Restrict
            entity.HasOne(c => c.CardOrder)
                .WithMany(co => co.Cards)
                .HasForeignKey(c => c.CardOrderId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔴 Restrict
            entity.HasOne(c => c.Owner)
                .WithMany(p => p.Cards)
                .HasForeignKey(c => c.OwnerPersonId)
                .OnDelete(DeleteBehavior.SetNull);  // 🟡 SetNull


            //راه 1 :
            //entity.HasIndex(c => c.CardNo).IsUnique().HasDatabaseName("IX_Cards_CardNo_Unique");
        });

        //راه 2:
        ////  ✅ ایندکس یونیک CardNo — جداگانه : 
        modelBuilder.Entity<Card>().HasIndex(c => c.CardNo).IsUnique().HasDatabaseName("IX_Cards_CardNo_Unique");

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            // User -> Company (چند به یک)
            entity.HasOne(u => u.Company)
                .WithMany()//این یعنی "Company چندین User دارد، اما من نیازی به پراپرتی Users در کلاس Company ندارم". این کار از نظر DDD و Clean Architecture صحیح‌تر است
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)// 🔴 Restrict  وگرنه با حذف شرکت ، یوزرهای آن نیز حذف می شوند // 
                                                  //Because default is cascade in SQL Server.
                .IsRequired();
            // یا اگر می‌خواهید ایندکس هم بسازید:
            //entity.HasIndex(u => u.CompanyId)
            //.HasDatabaseName("IX_Users_CompanyId");

            // تنظیمات دیگر User...
            //entity.Property(e => e.UserName).HasMaxLength(50);
            //entity.Property(e => e.NormalizedUserName).HasMaxLength(50);
            //entity.Property(e => e.Email).HasMaxLength(50);
            //entity.Property(e => e.NormalizedEmail).HasMaxLength(50);
            //entity.Property(e => e.PasswordHash).HasColumnType("nvarchar(656)");
            //entity.Property(e => e.SecurityStamp).HasColumnType("nvarchar(656)");
            //entity.Property(e => e.ConcurrencyStamp).HasColumnType("nvarchar(656)");
            //entity.Property(e => e.PhoneNumber).HasColumnType("varchar(20)");
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Id)
                 .ValueGeneratedOnAdd(); // EF این رو به identity تبدیل می‌کنه
            entity.Property(e => e.UserName)
                  .HasMaxLength(50);

            entity.Property(e => e.NormalizedUserName)
                  .HasMaxLength(50);

            entity.Property(e => e.Email)
                  .HasMaxLength(50);

            entity.Property(e => e.NormalizedEmail)
                  .HasMaxLength(50);

            entity.Property(e => e.PasswordHash)
                  .HasColumnType("nvarchar(656)");//? 256 is enough= 64 chars in program(الگوریتم PBKDF2 با HMAC-SHA256)

            entity.Property(e => e.SecurityStamp)
                  .HasColumnType("nvarchar(656)");//256 is enough

            entity.Property(e => e.ConcurrencyStamp)//256 is enough
                  .HasColumnType("nvarchar(656)");

            entity.Property(e => e.PhoneNumber)
                  .HasColumnType("varchar(20)");
            entity.Property(c => c.IsActive).HasDefaultValue(true);
        });
        modelBuilder.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = 1, Name = "admin", NormalizedName = "ADMIN" },
            new ApplicationRole { Id = 2, Name = "companyAdmin", NormalizedName = "COMPANYADMIN" },
            new ApplicationRole { Id = 3, Name = "companyUser", NormalizedName = "COMPANYUser" }
            );

        // یک کاربر
        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@givsoft.com",
            NormalizedEmail = "ADMIN@GIVSOFT.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            CompanyId = 1,//"11111111", "01307816"
            LastName = "admin",
            PhoneNumber = "09121307816",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            IsActive = true
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123"); // رمز پیش‌فرض

        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        // اتصال کاربر به نقش admin
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 1
            }
        );
        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(c => c.Status).HasComment(@$" 
                    {(byte)CardStatus.NewOrInitial}:{(CardStatus.NewOrInitial).GetDisplayAttributeValue()}
                    {(byte)CardStatus.Verified}:{(CardStatus.Verified).GetDisplayAttributeValue()}
                    {(byte)CardStatus.Canceled}:{(CardStatus.Canceled).GetDisplayAttributeValue()}");

        });
        modelBuilder.Entity<CardTransaction>(entity =>
        {


            entity.HasOne(ct => ct.Card)
            .WithMany(c => c.CardTransactions)
            .HasForeignKey(ct => ct.CardId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ct => ct.ProviderId)
                 //.IsUnique()
                 .HasDatabaseName("UX_CardTransaction_ProviderId");


            entity.Property(ct => ct.Status).HasComment(@$" 
                {(byte)CardTransactionsStatus.NewOrInitial}:{(CardTransactionsStatus.NewOrInitial).GetDisplayAttributeValue()}
                {(byte)CardTransactionsStatus.Verified}:{(CardTransactionsStatus.Verified).GetDisplayAttributeValue()} 
                {(byte)CardTransactionsStatus.Canceled_timedout}:{(CardTransactionsStatus.Canceled_timedout).GetDisplayAttributeValue()} 
                {(byte)CardTransactionsStatus.Canceled_Returned}:{(CardTransactionsStatus.Canceled_Returned).GetDisplayAttributeValue()}");

        });

        modelBuilder.Entity<CardOrder>(entity =>
        {
            entity.Property(ct => ct.Status).HasComment(@$"
                    {(byte)CardOrderStatus.NewOrInitial}:{(CardOrderStatus.NewOrInitial).GetDisplayAttributeValue()} 
                    {(byte)CardOrderStatus.Verified}:{(CardOrderStatus.Verified).GetDisplayAttributeValue()}
                    {(byte)CardOrderStatus.Canceled}:{(CardOrderStatus.Canceled).GetDisplayAttributeValue()} ");

        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(l => l.OperationId).
                HasComment(@$"
                        {(byte)LogOperationIdDescription.ConfirmCardOrder}:{(LogOperationIdDescription.ConfirmCardOrder).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.CancelCardOrder}:{(LogOperationIdDescription.CancelCardOrder).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.DisableCard}:{(LogOperationIdDescription.DisableCard).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.EnableCard}:{(LogOperationIdDescription.EnableCard).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.SetCardOwner}:{(LogOperationIdDescription.SetCardOwner).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.IncCardCredit}:{(LogOperationIdDescription.IncCardCredit).GetDescriptionAttributeValue()} 
                        {(byte)LogOperationIdDescription.DecCardCredit}:{(LogOperationIdDescription.DecCardCredit).GetDescriptionAttributeValue()} 
                        ");
        });


        modelBuilder.Entity<CardTransactionType>()
            .HasMany(ctt => ctt.CardTransactions)
            .WithOne(ct => ct.CardTransactionType)
            .HasForeignKey(ct => ct.CardTransactionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CardTransactionType>().HasData(
            new CardTransactionType { Id = 1, Title = "شارژ اولیه", Sign = 1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 2, Title = "خرج کردن", Sign = -1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 3, Title = "افزایش دستی اعتبار کارت ", Sign = +1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 4, Title = "کاهش دستی اعتبار کارت ", Sign = -1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 5, Title = "افزایش تراکنش لغو شده", Sign = 1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 6, Title = "افزایش تراکنش منقضی شده", Sign = 1, DateCreated = DateTime.Now, UserIdCreated = 1 },
            new CardTransactionType { Id = 7, Title = "افزایش اعتبار کارت از طریق Api", Sign = 1, DateCreated = DateTime.Now, UserIdCreated = 1 }
        );

        modelBuilder.Entity<CardTransactionType>().Property(p => p.Sign).HasDefaultValue(+1);
        modelBuilder.Entity<CardTransactionType>().Property(x => x.Id).ValueGeneratedNever();


        modelBuilder.Entity<Organization>().ToTable("Organizations");
        modelBuilder.Entity<Organization>()
            .HasIndex(p => p.OrganizationName)
            .IsUnique()
            .HasDatabaseName("UI_Organization_OrganizationName");

        modelBuilder.Entity<CardType>().HasData(
            new CardType
            {
                Id = 1,
                Title = "کارت هدیه",
                IsChargeable = false,
                DateCreated = DateTime.Now,
                UserIdCreated = 1
            },
            new CardType
            {
                Id = 2,
                Title = "کارت باشگاه مشتریان ",
                IsChargeable = false,
                DateCreated = DateTime.Now,
                UserIdCreated = 1
            }//loyalty card
        );


        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasIndex(e => new { e.CompanyId, e.Mobile })
                  .HasDatabaseName("X_Company_Mobile");
            entity.HasIndex(p => new { p.GivId, p.CompanyId })
                  .IsUnique()
                  .HasDatabaseName("UX_GivId_CompanyId");
            entity.Property(p => p.GivId).HasComment("UX_GivId_CompanyId: GivId is unique per company" + "\n" + "X_Company_Mobile: (GivId, CompanyId, Mobile) are indexed. For every company must be ONE Mobile registered and given one givId , but with api and offline data gathered its violation is possible.");
        });

       
        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 2, ParentId = null, Name = "UserAccount", Label = "مدیریت کاربران" },
            new Permission { Id = 3, ParentId = null, Name = "Company", Label = "مدیریت شرکت ها" },
            new Permission { Id = 4, ParentId = null, Name = "Organization", Label = "مدیریت سازمان ها" },
            new Permission { Id = 5, ParentId = null, Name = "Person", Label = "مدیریت شخص ها" },
            new Permission { Id = 6, ParentId = null, Name = "CardOrder", Label = "مدیریت سفارش کارت ها" },

            new Permission { Id = 16, ParentId = 3, Name = "CompanyList", Label = "شرکت ها" },
            new Permission { Id = 17, ParentId = 3, Name = "CompanyNew", Label = "شرکت جدید" },
            new Permission { Id = 18, ParentId = 2, Name = "UserAccountNew", Label = "کاربر جدید" },
            new Permission { Id = 19, ParentId = 2, Name = "UserAccountList", Label = "کاربران" },
            new Permission { Id = 20, ParentId = 2, Name = "UserAccountPermissions", Label = "مجوزها" },

            new Permission { Id = 21, ParentId = 4, Name = "OrganizationList", Label = " سازمان ها  " },
            new Permission { Id = 22, ParentId = 4, Name = "OrganizationView", Label = " مشاهده سازمان ها  " },
            new Permission { Id = 23, ParentId = 4, Name = "OrganizationNew", Label = " سازمان جدید  " },
            new Permission { Id = 24, ParentId = 4, Name = "OrganizationEdit", Label = "ویرایش سازمان" },

            new Permission { Id = 25, ParentId = 5, Name = "PersonList", Label = "شخص ها" },
            new Permission { Id = 26, ParentId = 5, Name = "PersonView", Label = "مشاهده شخص ها" },
            new Permission { Id = 27, ParentId = 5, Name = "PersonNew", Label = "شخص جدید" },
            new Permission { Id = 28, ParentId = 5, Name = "PersonEdit", Label = "ویرایش شخص" },

            new Permission { Id = 29, ParentId = 6, Name = "CardOrderList", Label = "سفارش کارت ها" },
            new Permission { Id = 30, ParentId = 6, Name = "CardOrderDetailsView", Label = "مشاهده جزییات سفارش کارت" },
            new Permission { Id = 31, ParentId = 6, Name = "CardOrderNew", Label = " سفارش کارت جدید" },
            new Permission { Id = 32, ParentId = 6, Name = "CardOrderEdit", Label = "ویرایش سفارش کارت" },
            new Permission { Id = 33, ParentId = 6, Name = "CardOrderConfirm", Label = " تایید سفارش کارت" },
            new Permission { Id = 34, ParentId = 6, Name = "CardOrderCancel", Label = " لغو سفارش کارت ها" },
            new Permission { Id = 35, ParentId = 6, Name = "CardOrderEnable", Label = "فعال کردن سفارش کارت" },
            new Permission { Id = 36, ParentId = 6, Name = "CardOrderDisable", Label = "غیر فعال کردن سفارش کارت" },
            new Permission { Id = 37, ParentId = 30, Name = "CardOrderSendToExcel", Label = "خروجی اکسل کارت ها " },
            new Permission { Id = 38, ParentId = 30, Name = "CardOrderCSV", Label = " خروجی csv " },
            new Permission { Id = 40, ParentId = 30, Name = "CardOrderSetAllCardsExpireDate", Label = " تعیین تاریخ اعتبار دسته جمعی کارت ها" },
            new Permission { Id = 41, ParentId = 30, Name = "CardOrderSetTheCardExpireDate", Label = " تعیین تاریخ اعتبار یک کارت" },
            new Permission { Id = 42, ParentId = 30, Name = "CardOrderViewTheCardTransactions", Label = "مشاهده تراکنش های کارت" },
            new Permission { Id = 43, ParentId = 30, Name = "CardOrderEnableTheCard", Label = "فعال کردن کارت" },
            new Permission { Id = 44, ParentId = 30, Name = "CardOrderDisableTheCard", Label = "غیر فعال کردن کارت " },
            new Permission { Id = 45, ParentId = 30, Name = "CardOrderSetOwnerOfTheCard", Label = "تعیین مالک کارت" },
            new Permission { Id = 46, ParentId = 30, Name = "CardOrderIncreaseCreditOfTheCard", Label = "افزایش اعتبار کارت" },
            new Permission { Id = 47, ParentId = 30, Name = "CardOrderDecreaseCreditOfTheCard", Label = "کاهش اعتبار کارت" },
            new Permission { Id = 48, ParentId = 3, Name = "CompanyEdit", Label = " ویرایش شرکت " },
            new Permission { Id = 49, ParentId = 2, Name = "UserAccountEdit", Label = " ویرایش کاربر " }
        
            );

        modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = 1, ParentId = null, Name = "Home", Label = "صفحه نخست", PermissionId = null, Controller = "Home", Action = "IndexNew" },
            new Menu { Id = 2, ParentId = null, Name = "UserAccount", Label = "مدیریت کاربران", PermissionId = 2 },
            new Menu { Id = 3, ParentId = null, Name = "Companya", Label = "مدیریت شرکت ها", PermissionId = 3 },
            new Menu { Id = 4, ParentId = null, Name = "Organization", Label = "مدیریت سازمان ها", PermissionId = 4 },
            new Menu { Id = 5, ParentId = null, Name = "Person", Label = "مدیریت شخص ها", PermissionId = 5 },
            new Menu { Id = 6, ParentId = null, Name = "CardOrder", Label = "مدیریت سفارش کارت ها", PermissionId = 6 },
            new Menu { Id = 7, ParentId = null, Name = "CardOrderDetailsView", Label = "جستجوی کارت ها", PermissionId = 30, Controller = "CardsManagement", Action = "CardsList" },

            new Menu { Id = 20, ParentId = 2, Name = "UserAccountNew", Label = "کاربر جدید", PermissionId = 18, Controller = "UserAccount", Action = "Create" },
            new Menu { Id = 21, ParentId = 2, Name = "UserAccountList", Label = "کاربران", PermissionId = 19, Controller = "UserAccount", Action = "Index" },
            new Menu { Id = 22, ParentId = 2, Name = "UserAccountPermissionGroups", Label = "گروه مجوزها", PermissionId = 20, Controller = "UserAccount", Action = "PermissionGroupIndex" },

            new Menu { Id = 23, ParentId = 3, Name = "CompanyNew", Label = "شرکت جدید", PermissionId = 17, Controller = "Company", Action = "Create" },
            new Menu { Id = 24, ParentId = 3, Name = "CompanyList", Label = "شرکت ها", PermissionId = 16, Controller = "Company", Action = "Index" },

            new Menu { Id = 25, ParentId = 4, Name = "OrganizationNew", Label = "سازمان جدید", PermissionId = 23, Controller = "Organization", Action = "Create" },
            new Menu { Id = 26, ParentId = 4, Name = "OrganizationList", Label = "سازمان ها", PermissionId = 21, Controller = "Organization", Action = "Index" },

            new Menu { Id = 27, ParentId = 5, Name = "PersonNew", Label = "شخص جدید", PermissionId = 27, Controller = "Person", Action = "Create" },
            new Menu { Id = 28, ParentId = 5, Name = "PersonList", Label = "شخص ها", PermissionId = 25, Controller = "Person", Action = "Index" },

            new Menu { Id = 29, ParentId = 6, Name = "CardOrderNew", Label = "سفارش کارت جدید", PermissionId = 31, Controller = "CardsManagement", Action = "CardOrderCreate" },
            new Menu { Id = 30, ParentId = 6, Name = "CardOrderList", Label = "سفارش کارت ها", PermissionId = 29, Controller = "CardsManagement", Action = "CardOrderIndex" }
                            );

        modelBuilder.Entity<Menu>()
            .HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Permission>()
            .HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasOne<ApplicationUser>()// سمت دیگر رابطه (بدون navigation property در UserPermission)
            .WithMany(u => u.UserPermissions)// collection navigation در ApplicationUser
            .HasForeignKey(up => up.UserId)//fk
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Permission>(up => up.Permission)  // navigation property در UserPermission
            .WithMany(p=>p.UserPermissions)//() if NO navigation  I have set
            .HasForeignKey(up => up.PermissionId)//fk
            .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<UserPermissiongroup>(entity =>
        {
            entity.HasOne<ApplicationUser>()
            .WithMany(user => user.UserPermissionGroups)
            .HasForeignKey(upg => upg.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Permissiongroup)//entity.HasOne<Permissiongroup>() // رابطه را باید مشخص کنی وگرنه یکی هم تکراری ایجاد میکنه
                .WithMany(p => p.UserPermissiongroups)
                .HasForeignKey(x => x.PermissiongroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });
       
        modelBuilder.Entity<PermissiongroupPermission>().HasKey(x => new { x.PermissionId, x.PermissionGroupId });

        modelBuilder.Entity<PermissiongroupPermission>()
            .HasOne(x => x.Permission)
            .WithMany(x => x.PermissiongroupPermissions)
            .HasForeignKey(x => x.PermissionId);

        modelBuilder.Entity<PermissiongroupPermission>()
            .HasOne(x => x.PermissionGroup)
            .WithMany(x => x.PermissiongroupPermissions)
            .HasForeignKey(x => x.PermissionGroupId);

        modelBuilder.Entity<UserPermission>().HasIndex(x => new{ x.UserId, x.PermissionId }).IsUnique();
        modelBuilder.Entity<UserPermissiongroup>().HasIndex(x => new{ x.UserId, x.PermissiongroupId }).IsUnique();
    }
}

