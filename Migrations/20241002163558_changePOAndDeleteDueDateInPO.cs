using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class changePOAndDeleteDueDateInPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrder_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrder_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser3",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiredDate",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser3",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                schema: "dbo",
                table: "OrdPurchaseOrder");

            migrationBuilder.AddColumn<Guid>(
                name: "DueDateId",
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

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "DueDateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrder_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseOrder",
                column: "DueDateId");

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
                name: "FK_OrdPurchaseRequest_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "DueDateId",
                principalSchema: "dbo",
                principalTable: "MstDueDate",
                principalColumn: "DueDateId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
