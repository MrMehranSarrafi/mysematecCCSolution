using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addDescriptionToPermissionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10b553fa-fbab-4022-9419-c2a54719adaf", "AQAAAAIAAYagAAAAEHsBXrZUt5PpTuvNXVMco35oG8slQaiJbXJxWca0HpC4BQcizZXFNy7qi+e9kUm0AA==", "75a794b7-bb2e-489e-854b-17df2d3fba6e" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9871));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9876));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9879));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9881));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9884));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9886));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 561, DateTimeKind.Local).AddTicks(9889));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 562, DateTimeKind.Local).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 562, DateTimeKind.Local).AddTicks(3815));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 14, 54, 27, 442, DateTimeKind.Local).AddTicks(8925));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "556d22d4-56a6-4286-a7aa-44e42382ccce", "AQAAAAIAAYagAAAAEGVr4bdse4wsL2bSlaLIVgtII180SkEmM1n2Qniy7nEq/nbisSxieFbXbOAcIdZ96A==", "dfb4cc6e-ba7f-4b48-9c5c-921fa536dcdc" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5778));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5782));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5785));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5787));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5790));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5792));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(5794));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(7986));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 29, 47, DateTimeKind.Local).AddTicks(7990));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 21, 11, 47, 28, 931, DateTimeKind.Local).AddTicks(3934));
        }
    }
}
