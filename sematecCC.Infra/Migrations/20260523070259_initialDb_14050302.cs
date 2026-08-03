using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class initialDb_14050302 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardTransactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sign = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsChargeable = table.Column<bool>(type: "bit", nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    ApiUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApiPassword = table.Column<string>(type: "nvarchar(656)", maxLength: 656, nullable: true),
                    ClientID = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateDone = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ObjectName = table.Column<string>(type: "varchar(50)", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OperationId = table.Column<byte>(type: "tinyint", nullable: false, comment: "\r\n1:تایید سفارش کارت \r\n2:لغو سفارش کارت \r\n3:غیرفعال کردن کارت \r\n4:فعال کردن کارت \r\n5:تعیین مالک کارت \r\n6: افزایش دستی اعتبار کارت \r\n7: کاهش دستی اعتبار کارت \r\n")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telephone = table.Column<string>(type: "varchar(20)", nullable: true),
                    Mobile = table.Column<string>(type: "varchar(20)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telephone = table.Column<string>(type: "varchar(20)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(656)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(656)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(656)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    NationalCode = table.Column<string>(type: "varchar(13)", nullable: true),
                    Mobile = table.Column<string>(type: "varchar(20)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: true),
                    JobPlace = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    BirthDateFa = table.Column<string>(type: "varchar(10)", nullable: true),
                    GivId = table.Column<long>(type: "bigint", nullable: false, comment: "UX_GivId_CompanyId: GivId is unique per company\nX_Company_Mobile: (GivId, CompanyId, Mobile) are indexed. For every company must be ONE Mobile registered and given one givId , but with api and offline data gathered its violation is possible."),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Tedad = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, comment: "\r\n1:اولیه \r\n2:تایید شده\r\n3:لغو شده "),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExpireDayNumber = table.Column<int>(type: "int", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpireDateFa = table.Column<string>(type: "varchar(10)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardOrders_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RemainedAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SerialNo = table.Column<string>(type: "varchar(7)", nullable: false),
                    CardNo = table.Column<string>(type: "varchar(16)", nullable: false),
                    Password = table.Column<string>(type: "varchar(5)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, comment: " \r\n1:اولیه\r\n2:تایید شده\r\n3:لغو شده"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpireDateFa = table.Column<string>(type: "varchar(10)", nullable: true),
                    CardOrderId = table.Column<int>(type: "int", nullable: false),
                    CardTypeId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    OwnerPersonId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_CardOrders_CardOrderId",
                        column: x => x.CardOrderId,
                        principalTable: "CardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cards_Persons_OwnerPersonId",
                        column: x => x.OwnerPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CardTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RemainedAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, comment: " \r\n1:اولیه\r\n2:تایید شده \r\n3:منقضی شده \r\n4:مرجوع شده"),
                    Description = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    ProviderId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    CardTransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTransactions_CardTransactionTypes_CardTransactionTypeId",
                        column: x => x.CardTransactionTypeId,
                        principalTable: "CardTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardTransactions_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, "admin", "ADMIN" },
                    { 2, null, "companyAdmin", "COMPANYADMIN" },
                    { 3, null, "companyUser", "COMPANYUser" }
                });

            migrationBuilder.InsertData(
                table: "CardTransactionTypes",
                columns: new[] { "Id", "DateChanged", "DateCreated", "Sign", "Title", "UserIdChanged", "UserIdCreated" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9901), (short)1, "شارژ اولیه", null, 1 },
                    { 2, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9904), (short)-1, "خرج کردن", null, 1 },
                    { 3, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9906), (short)1, "افزایش دستی اعتبار کارت ", null, 1 },
                    { 4, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9907), (short)-1, "کاهش دستی اعتبار کارت ", null, 1 },
                    { 5, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9909), (short)1, "افزایش تراکنش لغو شده", null, 1 },
                    { 6, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9911), (short)1, "افزایش تراکنش منقضی شده", null, 1 },
                    { 7, null, new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9913), (short)1, "افزایش اعتبار کارت از طریق Api", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "CardTypes",
                columns: new[] { "Id", "DateChanged", "DateCreated", "IsChargeable", "Title", "UserIdChanged", "UserIdCreated" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 5, 23, 10, 32, 58, 817, DateTimeKind.Local).AddTicks(1563), false, "کارت هدیه", null, 1 },
                    { 2, null, new DateTime(2026, 5, 23, 10, 32, 58, 817, DateTimeKind.Local).AddTicks(1565), false, "کارت باشگاه مشتریان ", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "ApiPassword", "ApiUsername", "ClientID", "ClientSecret", "CompanyCode", "CompanyName", "DateChanged", "DateCreated", "UserIdChanged", "UserIdCreated" },
                values: new object[] { 1, null, null, null, null, "01307816", "شرکت گیو", null, new DateTime(2026, 5, 23, 10, 32, 58, 755, DateTimeKind.Local).AddTicks(5164), null, 1 });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "DateChanged", "DateCreated", "FirstName", "LastName", "NationalCode", "UserIdChanged", "UserIdCreated" },
                values: new object[] { 1, null, new DateTime(2026, 5, 23, 10, 32, 58, 755, DateTimeKind.Local).AddTicks(4988), "مهران", "صرافی", "1234567890", null, 1 });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "CompanyId", "ConcurrencyStamp", "Description", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Telephone", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, 1, "991fb354-16d2-40e8-918d-4b7af7d8ba91", null, "admin@givsoft.com", true, null, true, "admin", false, null, "ADMIN@GIVSOFT.COM", "ADMIN", "AQAAAAIAAYagAAAAEOCYUC6sO534Zgchx65AABYW4HQJ3xxShUGPblk8RGHrpq/o+7idVuwAmcyn5xA82Q==", "09121307816", true, "a0dc8c2f-fe56-4c6b-9680-46f2ad21d8ca", null, false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CardOrders_CompanyId",
                table: "CardOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardOrders_OrganizationId",
                table: "CardOrders",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardNo_Unique",
                table: "Cards",
                column: "CardNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardOrderId",
                table: "Cards",
                column: "CardOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardTypeId",
                table: "Cards",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CompanyId",
                table: "Cards",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_OrganizationId",
                table: "Cards",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_OwnerPersonId",
                table: "Cards",
                column: "OwnerPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId",
                table: "CardTransactions",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardTransactionTypeId",
                table: "CardTransactions",
                column: "CardTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_CardTransaction_ProviderId",
                table: "CardTransactions",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyCode_Unique",
                table: "Companies",
                column: "CompanyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyName_Unique",
                table: "Companies",
                column: "CompanyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UI_Organization_OrganizationName",
                table: "Organizations",
                column: "OrganizationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_GivId_CompanyId",
                table: "Persons",
                columns: new[] { "GivId", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "X_Company_Mobile",
                table: "Persons",
                columns: new[] { "CompanyId", "Mobile" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CardTransactions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CardTransactionTypes");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CardOrders");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
