using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeRequestCancelOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrdRequestCancelOrder",
                schema: "dbo",
                columns: table => new
                {
                    RequestCancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCancelOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestCancelApproveById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestCancelApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_OrdRequestCancelOrder", x => x.RequestCancelOrderId);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                        column: x => x.RequestCancelApproveById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_MstUserActive_HeadPurchasingManagerId",
                        column: x => x.HeadPurchasingManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_MstUserActive_HeadWarehouseManagerId",
                        column: x => x.HeadWarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdRequestCancelOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    RequestCancelOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_OrdRequestCancelOrderDetail", x => x.RequestCancelOrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrderDetail_OrdRequestCancelOrder_RequestCancelOrderId",
                        column: x => x.RequestCancelOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdRequestCancelOrder",
                        principalColumn: "RequestCancelOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrderDetail_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "RequestCancelOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_HeadPurchasingManagerId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_HeadWarehouseManagerId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "HeadWarehouseManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_PurchaseOrderId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrderDetail_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdRequestCancelOrderDetail",
                column: "RequestCancelOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrderDetail_OrdRequestCancelOrder_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "RequestCancelOrderId",
                principalSchema: "dbo",
                principalTable: "OrdRequestCancelOrder",
                principalColumn: "RequestCancelOrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_OrdRequestCancelOrder_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "OrdRequestCancelOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrdRequestCancelOrder",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrderDetail_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");
        }
    }
}
