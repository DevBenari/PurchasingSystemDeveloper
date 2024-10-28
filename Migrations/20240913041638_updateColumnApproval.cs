using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateColumnApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropIndex(
                name: "IX_OrdApproval_UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropIndex(
                name: "IX_OrdApproval_UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "ApproveByUser1",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "ApproveByUser2",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "ApproveByUser3",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "User1ApproveDate",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "User1ApproveTime",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "User2ApproveDate",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.RenameColumn(
                name: "UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval",
                newName: "UserApproveId");

            migrationBuilder.RenameColumn(
                name: "User3ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveTime");

            migrationBuilder.RenameColumn(
                name: "User3ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveDate");

            migrationBuilder.RenameColumn(
                name: "User2ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveBy");

            migrationBuilder.RenameIndex(
                name: "IX_OrdApproval_UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval",
                newName: "IX_OrdApproval_UserApproveId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApproveId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApproveId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApproveId",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.RenameColumn(
                name: "UserApproveId",
                schema: "dbo",
                table: "OrdApproval",
                newName: "UserApprove3Id");

            migrationBuilder.RenameColumn(
                name: "ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "User3ApproveTime");

            migrationBuilder.RenameColumn(
                name: "ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "User3ApproveDate");

            migrationBuilder.RenameColumn(
                name: "ApproveBy",
                schema: "dbo",
                table: "OrdApproval",
                newName: "User2ApproveTime");

            migrationBuilder.RenameIndex(
                name: "IX_OrdApproval_UserApproveId",
                schema: "dbo",
                table: "OrdApproval",
                newName: "IX_OrdApproval_UserApprove3Id");

            migrationBuilder.AddColumn<string>(
                name: "ApproveByUser1",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApproveByUser2",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApproveByUser3",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "User1ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "User1ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "User2ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprove1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprove2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprove2Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprove3Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
