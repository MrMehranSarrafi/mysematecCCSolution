using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SematecCC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionGroupPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissionGroups_UserId",
                table: "UserPermissionGroups");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PermissionGroups",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PermissionGroupPermission",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    PermissionGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroupPermission", x => new { x.PermissionId, x.PermissionGroupId });
                    table.ForeignKey(
                        name: "FK_PermissionGroupPermission_PermissionGroups_PermissionGroupId",
                        column: x => x.PermissionGroupId,
                        principalTable: "PermissionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionGroupPermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Action", "Controller" },
                values: new object[] { "CardsList", "CardsManagement" });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "Label", "Name" },
                values: new object[] { "PermissionGroups", "گروه مجوزها", "UserAccountPermissionGroups" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGroups_UserId_PermissionGroupId",
                table: "UserPermissionGroups",
                columns: new[] { "UserId", "PermissionGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ParentId",
                table: "Permissions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGroupPermission_PermissionGroupId",
                table: "PermissionGroupPermission",
                column: "PermissionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Permissions_ParentId",
                table: "Permissions",
                column: "ParentId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Permissions_ParentId",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "PermissionGroupPermission");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissionGroups_UserId_PermissionGroupId",
                table: "UserPermissionGroups");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ParentId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PermissionGroups");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be17c4f3-0c26-40e0-bbed-2e01c31453cc", "AQAAAAIAAYagAAAAEIb+9+7bDBmh9YjBEqUAQk23lAau6i5dgoQ7urihrN2N+l9ASQGh4lWHzUA22iRdDg==", "da26f5bd-e2db-4b05-a2c8-dec58a93bb1b" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9252));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9255));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9257));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9292));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9294));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9296));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 81, DateTimeKind.Local).AddTicks(9298));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 82, DateTimeKind.Local).AddTicks(2101));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 82, DateTimeKind.Local).AddTicks(2105));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 17, 19, 9, 0, 9, DateTimeKind.Local).AddTicks(7517));

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Action", "Controller" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "Label", "Name" },
                values: new object[] { "Permissions", "مجوزها", "UserAccountPermissions" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGroups_UserId",
                table: "UserPermissionGroups",
                column: "UserId");
        }
    }
}
