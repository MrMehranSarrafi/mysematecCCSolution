using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menu_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be009c97-3996-4fbc-9f57-0f7a6ad427df", "AQAAAAIAAYagAAAAEKsFW3v/MrSve7zhFmi/3y3qQBKh96r9gykj0qOwVzR1mmgB4ukOdDWS3zh2A5xocg==", "97d89992-4739-4aba-8f48-13c959125906" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3410));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3414));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3415));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3417));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3418));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3420));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(3422));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(5603));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 735, DateTimeKind.Local).AddTicks(5605));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 675, DateTimeKind.Local).AddTicks(634));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 42, 44, 675, DateTimeKind.Local).AddTicks(446));

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "Label", "Name", "ParentId", "PermissionId" },
                values: new object[,]
                {
                    { 1, "صفحه نخست", "Home", null, null },
                    { 2, "مدیریت کاربران", "UserAccount", null, 2 },
                    { 3, "مدیریت شرکت ها", "Company", null, 3 },
                    { 7, "جستجوی کارت ها", "CardOrderDetailsView", null, 30 },
                    { 25, "سازمان جدید", "OrganizationNew", 4, 23 },
                    { 26, "سازمان ها", "OrganizationList", 4, 21 },
                    { 27, "شخص جدید", "PersonNew", 5, 27 },
                    { 28, "شخص ها", "PersonList", 5, 25 },
                    { 29, "سفارش کارت جدید", "CardOrderNew", 6, 31 },
                    { 30, "سفارش کارت ها", "CardOrderList", 6, 29 }
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Label", "Name" },
                values: new object[] { "مدیریت کاربران", "UserAccount" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Label", "Name" },
                values: new object[] { "مدیریت شرکت ها", "Company" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " سازمان ها  ", 4 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " مشاهده سازمان ها  ", 4 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " سازمان جدید  ", 4 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "ParentId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "شخص ها", 5 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "ParentId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "شخص جدید", 5 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "ParentId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "سفارش کارت ها", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "مشاهده جزییات سفارش کارت", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " سفارش کارت جدید", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "ParentId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " تایید سفارش کارت", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " لغو سفارش کارت ها", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "ParentId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "غیر فعال کردن سفارش کارت", 6 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "Label",
                value: "خروجی اکسل کارت ها ");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "Label",
                value: " تعیین تاریخ اعتبار دسته جمعی کارت ها");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "Label",
                value: " تعیین تاریخ اعتبار یک کارت");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Label", "Name", "ParentId" },
                values: new object[,]
                {
                    { 4, "مدیریت سازمان ها", "Organization", null },
                    { 5, "مدیریت شخص ها", "Person", null },
                    { 6, "مدیریت سفارش کارت ها", "CardOrder", null },
                    { 16, "شرکت ها", "CompanyList", 3 },
                    { 17, "شرکت جدید", "CompanyNew", 3 },
                    { 18, "کاربر جدید", "UserAccountNew", 2 },
                    { 19, "کاربران", "UserAccountList", 2 },
                    { 20, "مجوزها", "UserAccountPermissions", 2 }
                });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "Label", "Name", "ParentId", "PermissionId" },
                values: new object[,]
                {
                    { 4, "مدیریت سازمان ها", "Organization", null, 4 },
                    { 5, "مدیریت شخص ها", "Person", null, 5 },
                    { 6, "مدیریت سفارش کارت ها", "CardOrder", null, 6 },
                    { 20, "کاربر جدید", "UserAccountNew", 2, 18 },
                    { 21, "کاربران", "UserAccountList", 2, 19 },
                    { 22, "مجوزها", "UserAccountPermissions", 2, 20 },
                    { 23, "شرکت جدید", "CompanyNew", 3, 17 },
                    { 24, "شرکت ها", "CompanyList", 3, 16 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menu_PermissionId",
                table: "Menu",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "91c1452a-9ab7-4696-9ea5-da018bf710e5", "AQAAAAIAAYagAAAAEEnxjGRAEyKK/uq4fdLW3ecaBSWN1+9TxEWINgPysZj+2Zl1C0QfFnozyOzHvBBsLA==", "2270c6cc-30dc-472e-8906-fb9c34ba6508" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9754));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9757));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9759));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9762));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9764));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9766));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 301, DateTimeKind.Local).AddTicks(9768));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 302, DateTimeKind.Local).AddTicks(1489));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 302, DateTimeKind.Local).AddTicks(1491));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 243, DateTimeKind.Local).AddTicks(1668));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 11, 13, 0, 243, DateTimeKind.Local).AddTicks(1431));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Label", "Name" },
                values: new object[] { "مدیریت شخص ها", "Person" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Label", "Name" },
                values: new object[] { "مدیریت سفارش کارت ها", "CardOrder" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "لیست سازمان ها", 1 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "مشاهده سازمان ها", 1 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "ایجاد سازمان جدید", 1 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "ParentId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "لیست شخص ها", 2 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "ParentId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "ایجاد شخص جدید", 2 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "ParentId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "لیست  سفارش کارت ها", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " مشاهده جزییات سفارش کارت", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "ایجاد سفارش کارت جدید", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "ParentId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "تایید سفارش کارت ", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { "لغو سفارش کارت ها", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "ParentId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Label", "ParentId" },
                values: new object[] { " غیر فعال کردن سفارش کارت", 3 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "Label",
                value: "خروجی اکسل کارت ها");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "Label",
                value: " تعیین تاریخ اعتبار دسته جمعی کارت ها ");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "Label",
                value: "تعیین تاریخ اعتبار یک کارت");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Label", "Name", "ParentId" },
                values: new object[] { 1, "مدیریت سازمان ها", "Organization", null });
        }
    }
}
