using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateRequestCancelOrderDeleteDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_OrdRequestCancelOrder_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder");

            migrationBuilder.DropTable(
                name: "OrdRequestCancelOrderDetail",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseOrderDetail_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrdRequestCancelOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    RequestCancelOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyCancel = table.Column<int>(type: "int", nullable: false),
                    QtyOrder = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseOrderDetail_RequestCancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "RequestCancelOrderId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
