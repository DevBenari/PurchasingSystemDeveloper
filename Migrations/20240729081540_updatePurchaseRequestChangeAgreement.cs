using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updatePurchaseRequestChangeAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserApprovalId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_UserApprovalId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserAgreementId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserAgreementId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
