using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addPermissionsAndMenuTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e0933329-ac0b-4769-8812-985605640f20", "AQAAAAIAAYagAAAAEOP8vZhSSedXz/lLXJzNGe9NQ5QapaU2Aq+G6f0f+gRqqsln9lhslEvQDPWGZTglYw==", "0bc52e40-c3a2-49da-ad0d-5ccaa2b98872" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2366));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2369));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2372));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2374));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2375));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(2377));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(4138));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 285, DateTimeKind.Local).AddTicks(4141));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 225, DateTimeKind.Local).AddTicks(2503));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 14, 15, 44, 37, 225, DateTimeKind.Local).AddTicks(2342));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
