using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializePurchaseRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdPurchaseRequest",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserAgreement1Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgreement2Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermOfPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyTotal = table.Column<int>(type: "int", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_OrdPurchaseRequest", x => x.PurchaseRequestId);
                    table.ForeignKey(
                        name: "FK_OrdPurchaseRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdPurchaseRequest_MstTermOfPayment_TermOfPaymentId",
                        column: x => x.TermOfPaymentId,
                        principalSchema: "dbo",
                        principalTable: "MstTermOfPayment",
                        principalColumn: "TermOfPaymentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdPurchaseRequestDetail",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseRequestDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Principal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_OrdPurchaseRequestDetail", x => x.PurchaseRequestDetailId);
                    table.ForeignKey(
                        name: "FK_OrdPurchaseRequestDetail_OrdPurchaseRequest_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseRequest",
                        principalColumn: "PurchaseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_TermOfPaymentId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "TermOfPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_UserAccessId",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequestDetail_PurchaseRequestId",
                schema: "dbo",
                table: "OrdPurchaseRequestDetail",
                column: "PurchaseRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdPurchaseRequestDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrdPurchaseRequest",
                schema: "dbo");
        }
    }
}
