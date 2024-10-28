using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeReceiveOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WrhReceiveOrder",
                schema: "dbo",
                columns: table => new
                {
                    ReceiveOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiveOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiveById = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_WrhReceiveOrder", x => x.ReceiveOrderId);
                    table.ForeignKey(
                        name: "FK_WrhReceiveOrder_AspNetUsers_ReceiveById",
                        column: x => x.ReceiveById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhReceiveOrder_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhReceiveOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    ReceivedOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiveOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyOrder = table.Column<int>(type: "int", nullable: false),
                    QtyReceive = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_WrhReceiveOrderDetail", x => x.ReceivedOrderDetailId);
                    table.ForeignKey(
                        name: "FK_WrhReceiveOrderDetail_WrhReceiveOrder_ReceiveOrderId",
                        column: x => x.ReceiveOrderId,
                        principalSchema: "dbo",
                        principalTable: "WrhReceiveOrder",
                        principalColumn: "ReceiveOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrderDetail_ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "ReceiveOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhReceiveOrder_PurchaseOrderId",
                schema: "dbo",
                table: "WrhReceiveOrder",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhReceiveOrder_ReceiveById",
                schema: "dbo",
                table: "WrhReceiveOrder",
                column: "ReceiveById");

            migrationBuilder.CreateIndex(
                name: "IX_WrhReceiveOrderDetail_ReceiveOrderId",
                schema: "dbo",
                table: "WrhReceiveOrderDetail",
                column: "ReceiveOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhReceiveOrder_ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "ReceiveOrderId",
                principalSchema: "dbo",
                principalTable: "WrhReceiveOrder",
                principalColumn: "ReceiveOrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhReceiveOrder_ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "WrhReceiveOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhReceiveOrder",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrderDetail_ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "ReceiveOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");
        }
    }
}
