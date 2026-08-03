using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardNoGenerator.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addSelfJoinToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Permissions_PermissionId",
                table: "Menu");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Menu",
                table: "Menu");

            migrationBuilder.RenameTable(
                name: "Menu",
                newName: "Menus");

            migrationBuilder.RenameIndex(
                name: "IX_Menu_PermissionId",
                table: "Menus",
                newName: "IX_Menus_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Menus",
                table: "Menus",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Menus_ParentId",
                table: "Menus",
                column: "ParentId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Permissions_PermissionId",
                table: "Menus",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Menus_ParentId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Permissions_PermissionId",
                table: "Menus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Menus",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_ParentId",
                table: "Menus");

            migrationBuilder.RenameTable(
                name: "Menus",
                newName: "Menu");

            migrationBuilder.RenameIndex(
                name: "IX_Menus_PermissionId",
                table: "Menu",
                newName: "IX_Menu_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Menu",
                table: "Menu",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateChanged = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "GETDATE()"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UserIdChanged = table.Column<int>(type: "int", nullable: true),
                    UserIdCreated = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

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

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "DateChanged", "DateCreated", "FirstName", "LastName", "NationalCode", "UserIdChanged", "UserIdCreated" },
                values: new object[] { 1, null, new DateTime(2026, 6, 14, 15, 44, 37, 225, DateTimeKind.Local).AddTicks(2342), "مهران", "صرافی", "1234567890", null, 1 });

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Permissions_PermissionId",
                table: "Menu",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id");
        }
    }
}
