using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeCancelOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WrhCancelOrder",
                schema: "dbo",
                columns: table => new
                {
                    CancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelById = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_WrhCancelOrder", x => x.CancelOrderId);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_AspNetUsers_CancelById",
                        column: x => x.CancelById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhCancelOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    CancelOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyOrder = table.Column<int>(type: "int", nullable: false),
                    QtyCancel = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_WrhCancelOrderDetail", x => x.CancelOrderDetailId);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrderDetail_WrhCancelOrder_CancelOrderId",
                        column: x => x.CancelOrderId,
                        principalSchema: "dbo",
                        principalTable: "WrhCancelOrder",
                        principalColumn: "CancelOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrderDetail_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "CancelOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_CancelById",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "CancelById");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_PurchaseOrderId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrderDetail_CancelOrderId",
                schema: "dbo",
                table: "WrhCancelOrderDetail",
                column: "CancelOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhCancelOrder_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "CancelOrderId",
                principalSchema: "dbo",
                principalTable: "WrhCancelOrder",
                principalColumn: "CancelOrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhCancelOrder_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "WrhCancelOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhCancelOrder",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrderDetail_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");
        }
    }
}
