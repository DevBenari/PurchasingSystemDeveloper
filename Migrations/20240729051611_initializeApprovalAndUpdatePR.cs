using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeApprovalAndUpdatePR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgreement1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "UserAgreement2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrdApproval",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdApproval", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_OrdApproval_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApproval_MstUserActive_UserApprovalId",
                        column: x => x.UserApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApproval_OrdPurchaseRequest_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseRequest",
                        principalColumn: "PurchaseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_PurchaseRequestId",
                schema: "dbo",
                table: "OrdApproval",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserAccessId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserApprovalId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApprovalId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstUserActive_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropTable(
                name: "OrdApproval",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "UserAgreementId",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.AddColumn<string>(
                name: "UserAgreement1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgreement2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
