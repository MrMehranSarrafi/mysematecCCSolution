using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SematecCC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addControllerAndActionToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5730fe18-4ce4-4580-b736-e57e6a57f032", "AQAAAAIAAYagAAAAEMg7CpdVjdcxgUwk1ObhqHreKrOj4+SFz1ySZuxc7RnjKyFu74hhEQsFXXO4IOwQ8Q==", "2abe231a-ee70-490a-b239-8c5b35771dac" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5943));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5945));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5947));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5949));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5951));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5953));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(5954));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(7780));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 474, DateTimeKind.Local).AddTicks(7782));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 14, 17, 41, 412, DateTimeKind.Local).AddTicks(9680));

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "IndexNew", "Home" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Create", "UserAccount" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Index", "UserAccount" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Permissions", "UserAccount" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Create", "Company" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Index", "Company" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Create", "Organization" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Index", "Organization" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Create", "Person" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "Index", "Person" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "CardOrderCreate", "CardsManagement" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "CardOrderIndex", "CardsManagement" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Controller",
                table: "Menus");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3300f5ca-a983-4bcc-9d0d-ff27bd8eb6fd", "AQAAAAIAAYagAAAAEOhRCWICmfpJ5wgKtYUhZjxrDo4rRKPAfqIS+TBvvCGPLs8AIM8IAS5fTv4tLT0c4g==", "a6cfd2ad-205d-4a5b-a7d8-05d64196b942" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9913));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9915));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9917));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9919));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9921));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9923));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 798, DateTimeKind.Local).AddTicks(9925));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 799, DateTimeKind.Local).AddTicks(1666));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 799, DateTimeKind.Local).AddTicks(1669));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 16, 12, 48, 39, 738, DateTimeKind.Local).AddTicks(7970));
        }
    }
}
