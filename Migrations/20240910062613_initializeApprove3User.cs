using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeApprove3User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.RenameColumn(
                name: "UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                newName: "UserApprove3Id");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseRequest_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                newName: "IX_OrdPurchaseRequest_UserApprove3Id");

            migrationBuilder.RenameColumn(
                name: "UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                newName: "UserApprove3Id");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseOrder_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                newName: "IX_OrdPurchaseOrder_UserApprove3Id");

            migrationBuilder.RenameColumn(
                name: "UserApprovalId",
                schema: "dbo",
                table: "OrdApproval",
                newName: "UserApprove3Id");

            migrationBuilder.RenameColumn(
                name: "ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "User3ApproveDate");

            migrationBuilder.RenameColumn(
                name: "ApproveBy",
                schema: "dbo",
                table: "OrdApproval",
                newName: "User3ApproveTime");

            migrationBuilder.RenameIndex(
                name: "IX_OrdApproval_UserApprovalId",
                schema: "dbo",
                table: "OrdApproval",
                newName: "IX_OrdApproval_UserApprove3Id");

            migrationBuilder.AddColumn<Guid>(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddColumn<Guid>(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "User2ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
                name: "IX_OrdPurchaseRequest_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "DueDateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprove1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprove2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrder_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "DueDateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrder_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprove1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrder_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprove2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                column: "DueDateId");

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
                name: "FK_OrdApproval_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                column: "DueDateId",
                principalSchema: "dbo",
                principalTable: "MstDueDate",
                principalColumn: "DueDateId",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrder_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "DueDateId",
                principalSchema: "dbo",
                principalTable: "MstDueDate",
                principalColumn: "DueDateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprove2Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprove3Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "DueDateId",
                principalSchema: "dbo",
                principalTable: "MstDueDate",
                principalColumn: "DueDateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprove2Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprove3Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdApproval");

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

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrder_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrder_UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrder_UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrdApproval_DueDateId",
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
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "OrdPurchaseOrder");

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
                name: "DueDateId",
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
                name: "User2ApproveTime",
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
                table: "OrdPurchaseRequest",
                newName: "UserApprovalId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseRequest_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                newName: "IX_OrdPurchaseRequest_UserApprovalId");

            migrationBuilder.RenameColumn(
                name: "UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                newName: "UserApprovalId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseOrder_UserApprove3Id",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                newName: "IX_OrdPurchaseOrder_UserApprovalId");

            migrationBuilder.RenameColumn(
                name: "UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval",
                newName: "UserApprovalId");

            migrationBuilder.RenameColumn(
                name: "User3ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveBy");

            migrationBuilder.RenameColumn(
                name: "User3ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveDate");

            migrationBuilder.RenameIndex(
                name: "IX_OrdApproval_UserApprove3Id",
                schema: "dbo",
                table: "OrdApproval",
                newName: "IX_OrdApproval_UserApprovalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprovalId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrder_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "UserApprovalId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprovalId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
