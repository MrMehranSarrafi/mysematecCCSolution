using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SematecCC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class revisePermissiontableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionGroupPermission_PermissionGroups_PermissionGroupId",
                table: "PermissionGroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionGroupPermission_Permissions_PermissionId",
                table: "PermissionGroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionGroups_AspNetUsers_UserId",
                table: "UserPermissionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionGroups_PermissionGroups_PermissionGroupId",
                table: "UserPermissionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_AspNetUsers_UserId",
                table: "UserPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionId",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionGroups",
                table: "PermissionGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionGroupPermission",
                table: "PermissionGroupPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissionGroups",
                table: "UserPermissionGroups");

            migrationBuilder.RenameTable(
                name: "PermissionGroups",
                newName: "Permissiongroups");

            migrationBuilder.RenameTable(
                name: "PermissionGroupPermission",
                newName: "PermissiongroupPermission");

            migrationBuilder.RenameTable(
                name: "UserPermissions",
                newName: "UserPermission");

            migrationBuilder.RenameTable(
                name: "UserPermissionGroups",
                newName: "UserPermissiongroup");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionGroupPermission_PermissionGroupId",
                table: "PermissiongroupPermission",
                newName: "IX_PermissiongroupPermission_PermissionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermission",
                newName: "IX_UserPermission_UserId_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermission",
                newName: "IX_UserPermission_PermissionId");

            migrationBuilder.RenameColumn(
                name: "PermissionGroupId",
                table: "UserPermissiongroup",
                newName: "PermissiongroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionGroups_UserId_PermissionGroupId",
                table: "UserPermissiongroup",
                newName: "IX_UserPermissiongroup_UserId_PermissiongroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionGroups_PermissionGroupId",
                table: "UserPermissiongroup",
                newName: "IX_UserPermissiongroup_PermissiongroupId");

            migrationBuilder.AddColumn<int>(
                name: "PermissionId1",
                table: "UserPermission",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermissiongroupId1",
                table: "UserPermissiongroup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissiongroups",
                table: "Permissiongroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissiongroupPermission",
                table: "PermissiongroupPermission",
                columns: new[] { "PermissionId", "PermissionGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermission",
                table: "UserPermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissiongroup",
                table: "UserPermissiongroup",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4ce48bd5-6357-4dfa-b4f8-702231a7a0d2", "AQAAAAIAAYagAAAAEDYHN9/6JmE999nin/A7NmJWXxbKfEtFCersZL2tdieeohwneSasjKogaF3r5jBT1w==", "73396aa4-43c1-4c15-b33e-be7c69815971" });

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3669));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3673));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3675));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3678));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3680));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3682));

            migrationBuilder.UpdateData(
                table: "CardTransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(3684));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(5606));

            migrationBuilder.UpdateData(
                table: "CardTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 481, DateTimeKind.Local).AddTicks(5609));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 6, 28, 10, 46, 51, 398, DateTimeKind.Local).AddTicks(4672));

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 22,
                column: "Action",
                value: "PermissionGroupIndex");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_PermissionId1",
                table: "UserPermission",
                column: "PermissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissiongroup_PermissiongroupId1",
                table: "UserPermissiongroup",
                column: "PermissiongroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissiongroupPermission_Permissiongroups_PermissionGroupId",
                table: "PermissiongroupPermission",
                column: "PermissionGroupId",
                principalTable: "Permissiongroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissiongroupPermission_Permissions_PermissionId",
                table: "PermissiongroupPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_AspNetUsers_UserId",
                table: "UserPermission",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_Permissions_PermissionId",
                table: "UserPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_Permissions_PermissionId1",
                table: "UserPermission",
                column: "PermissionId1",
                principalTable: "Permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissiongroup_AspNetUsers_UserId",
                table: "UserPermissiongroup",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissiongroup_Permissiongroups_PermissiongroupId",
                table: "UserPermissiongroup",
                column: "PermissiongroupId",
                principalTable: "Permissiongroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissiongroup_Permissiongroups_PermissiongroupId1",
                table: "UserPermissiongroup",
                column: "PermissiongroupId1",
                principalTable: "Permissiongroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissiongroupPermission_Permissiongroups_PermissionGroupId",
                table: "PermissiongroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissiongroupPermission_Permissions_PermissionId",
                table: "PermissiongroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_AspNetUsers_UserId",
                table: "UserPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_Permissions_PermissionId",
                table: "UserPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_Permissions_PermissionId1",
                table: "UserPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissiongroup_AspNetUsers_UserId",
                table: "UserPermissiongroup");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissiongroup_Permissiongroups_PermissiongroupId",
                table: "UserPermissiongroup");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissiongroup_Permissiongroups_PermissiongroupId1",
                table: "UserPermissiongroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissiongroups",
                table: "Permissiongroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissiongroupPermission",
                table: "PermissiongroupPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissiongroup",
                table: "UserPermissiongroup");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissiongroup_PermissiongroupId1",
                table: "UserPermissiongroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermission",
                table: "UserPermission");

            migrationBuilder.DropIndex(
                name: "IX_UserPermission_PermissionId1",
                table: "UserPermission");

            migrationBuilder.DropColumn(
                name: "PermissiongroupId1",
                table: "UserPermissiongroup");

            migrationBuilder.DropColumn(
                name: "PermissionId1",
                table: "UserPermission");

            migrationBuilder.RenameTable(
                name: "Permissiongroups",
                newName: "PermissionGroups");

            migrationBuilder.RenameTable(
                name: "PermissiongroupPermission",
                newName: "PermissionGroupPermission");

            migrationBuilder.RenameTable(
                name: "UserPermissiongroup",
                newName: "UserPermissionGroups");

            migrationBuilder.RenameTable(
                name: "UserPermission",
                newName: "UserPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_PermissiongroupPermission_PermissionGroupId",
                table: "PermissionGroupPermission",
                newName: "IX_PermissionGroupPermission_PermissionGroupId");

            migrationBuilder.RenameColumn(
                name: "PermissiongroupId",
                table: "UserPermissionGroups",
                newName: "PermissionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissiongroup_UserId_PermissiongroupId",
                table: "UserPermissionGroups",
                newName: "IX_UserPermissionGroups_UserId_PermissionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissiongroup_PermissiongroupId",
                table: "UserPermissionGroups",
                newName: "IX_UserPermissionGroups_PermissionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermission_UserId_PermissionId",
                table: "UserPermissions",
                newName: "IX_UserPermissions_UserId_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermission_PermissionId",
                table: "UserPermissions",
                newName: "IX_UserPermissions_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionGroups",
                table: "PermissionGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionGroupPermission",
                table: "PermissionGroupPermission",
                columns: new[] { "PermissionId", "PermissionGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissionGroups",
                table: "UserPermissionGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions",
                column: "Id");

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

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 22,
                column: "Action",
                value: "PermissionGroups");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionGroupPermission_PermissionGroups_PermissionGroupId",
                table: "PermissionGroupPermission",
                column: "PermissionGroupId",
                principalTable: "PermissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionGroupPermission_Permissions_PermissionId",
                table: "PermissionGroupPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionGroups_AspNetUsers_UserId",
                table: "UserPermissionGroups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionGroups_PermissionGroups_PermissionGroupId",
                table: "UserPermissionGroups",
                column: "PermissionGroupId",
                principalTable: "PermissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_AspNetUsers_UserId",
                table: "UserPermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
