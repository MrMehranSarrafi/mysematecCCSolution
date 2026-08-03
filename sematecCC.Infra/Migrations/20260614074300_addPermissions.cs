using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "OperationId",
                table: "Logs",
                type: "tinyint",
                nullable: false,
                comment: "\r\n                        1:تایید سفارش کارت \r\n                        2:لغو سفارش کارت \r\n                        3:غیرفعال کردن کارت \r\n                        4:فعال کردن کارت \r\n                        5:تعیین مالک کارت \r\n                        6: افزایش دستی اعتبار کارت \r\n                        7: کاهش دستی اعتبار کارت \r\n                        ",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "\r\n1:تایید سفارش کارت \r\n2:لغو سفارش کارت \r\n3:غیرفعال کردن کارت \r\n4:فعال کردن کارت \r\n5:تعیین مالک کارت \r\n6: افزایش دستی اعتبار کارت \r\n7: کاهش دستی اعتبار کارت \r\n");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "CardTransactions",
                type: "tinyint",
                nullable: false,
                comment: " \r\n                1:اولیه\r\n                2:تایید شده \r\n                3:منقضی شده \r\n                4:مرجوع شده",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: " \r\n1:اولیه\r\n2:تایید شده \r\n3:منقضی شده \r\n4:مرجوع شده");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Cards",
                type: "tinyint",
                nullable: false,
                comment: " \r\n                    1:اولیه\r\n                    2:تایید شده\r\n                    3:لغو شده",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: " \r\n1:اولیه\r\n2:تایید شده\r\n3:لغو شده");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "CardOrders",
                type: "tinyint",
                nullable: false,
                comment: "\r\n                    1:اولیه \r\n                    2:تایید شده\r\n                    3:لغو شده ",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "\r\n1:اولیه \r\n2:تایید شده\r\n3:لغو شده ");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Label", "Name", "ParentId" },
                values: new object[,]
                {
                    { 1, "مدیریت سازمان ها", "Organization", null },
                    { 2, "مدیریت شخص ها", "Person", null },
                    { 3, "مدیریت سفارش کارت ها", "CardOrder", null },
                    { 21, "لیست سازمان ها", "OrganizationList", 1 },
                    { 22, "مشاهده سازمان ها", "OrganizationView", 1 },
                    { 23, "ایجاد سازمان جدید", "OrganizationNew", 1 },
                    { 24, "ویرایش سازمان", "OrganizationEdit", 1 },
                    { 25, "لیست شخص ها", "PersonList", 2 },
                    { 26, "مشاهده شخص ها", "PersonView", 2 },
                    { 27, "ایجاد شخص جدید", "PersonNew", 2 },
                    { 28, "ویرایش شخص", "PersonEdit", 2 },
                    { 29, "لیست  سفارش کارت ها", "CardOrderList", 3 },
                    { 30, " مشاهده جزییات سفارش کارت", "CardOrderDetailsView", 3 },
                    { 31, "ایجاد سفارش کارت جدید", "CardOrderNew", 3 },
                    { 32, "ویرایش سفارش کارت", "CardOrderEdit", 3 },
                    { 33, "تایید سفارش کارت ", "CardOrderConfirm", 3 },
                    { 34, "لغو سفارش کارت ها", "CardOrderCancel", 3 },
                    { 35, "فعال کردن سفارش کارت", "CardOrderEnable", 3 },
                    { 36, " غیر فعال کردن سفارش کارت", "CardOrderDisable", 3 },
                    { 37, "خروجی اکسل کارت ها", "CardOrderSendToExcel", 30 },
                    { 38, " خروجی csv ", "CardOrderCSV", 30 },
                    { 40, " تعیین تاریخ اعتبار دسته جمعی کارت ها ", "CardOrderSetAllCardsExpireDate", 30 },
                    { 41, "تعیین تاریخ اعتبار یک کارت", "CardOrderSetTheCardExpireDate", 30 },
                    { 42, "مشاهده تراکنش های کارت", "CardOrderViewTheCardTransactions", 30 },
                    { 43, "فعال کردن کارت", "CardOrderEnableTheCard", 30 },
                    { 44, "غیر فعال کردن کارت ", "CardOrderDisableTheCard", 30 },
                    { 45, "تعیین مالک کارت", "CardOrderSetOwnerOfTheCard", 30 },
                    { 46, "افزایش اعتبار کارت", "CardOrderIncreaseCreditOfTheCard", 30 },
                    { 47, "کاهش اعتبار کارت", "CardOrderDecreaseCreditOfTheCard", 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.AlterColumn<byte>(
                name: "OperationId",
                table: "Logs",
                type: "tinyint",
                nullable: false,
                comment: "\r\n1:تایید سفارش کارت \r\n2:لغو سفارش کارت \r\n3:غیرفعال کردن کارت \r\n4:فعال کردن کارت \r\n5:تعیین مالک کارت \r\n6: افزایش دستی اعتبار کارت \r\n7: کاهش دستی اعتبار کارت \r\n",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "\r\n                        1:تایید سفارش کارت \r\n                        2:لغو سفارش کارت \r\n                        3:غیرفعال کردن کارت \r\n                        4:فعال کردن کارت \r\n                        5:تعیین مالک کارت \r\n                        6: افزایش دستی اعتبار کارت \r\n                        7: کاهش دستی اعتبار کارت \r\n                        ");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "CardTransactions",
                type: "tinyint",
                nullable: false,
                comment: " \r\n1:اولیه\r\n2:تایید شده \r\n3:منقضی شده \r\n4:مرجوع شده",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: " \r\n                1:اولیه\r\n                2:تایید شده \r\n                3:منقضی شده \r\n                4:مرجوع شده");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Cards",
                type: "tinyint",
                nullable: false,
                comment: " \r\n1:اولیه\r\n2:تایید شده\r\n3:لغو شده",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: " \r\n                    1:اولیه\r\n                    2:تایید شده\r\n                    3:لغو شده");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "CardOrders",
                type: "tinyint",
                nullable: false,
                comment: "\r\n1:اولیه \r\n2:تایید شده\r\n3:لغو شده ",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "\r\n                    1:اولیه \r\n                    2:تایید شده\r\n                    3:لغو شده ");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "991fb354-16d2-40e8-918d-4b7af7d8ba91", "AQAAAAIAAYagAAAAEOCYUC6sO534Zgchx65AABYW4HQJ3xxShUGPblk8RGHrpq/o+7idVuwAmcyn5xA82Q==", "a0dc8c2f-fe56-4c6b-9680-46f2ad21d8ca" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9901));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9904));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9906));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9907));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9909));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9911));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 816, DateTimeKind.Local).AddTicks(9913));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 817, DateTimeKind.Local).AddTicks(1563));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 817, DateTimeKind.Local).AddTicks(1565));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 755, DateTimeKind.Local).AddTicks(5164));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 5, 23, 10, 32, 58, 755, DateTimeKind.Local).AddTicks(4988));
        }
    }
}
