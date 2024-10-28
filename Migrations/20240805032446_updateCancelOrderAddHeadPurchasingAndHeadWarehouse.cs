using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateCancelOrderAddHeadPurchasingAndHeadWarehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadWarehouseManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WrhCancelOrder_MstUserActive_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadPurchasingManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhCancelOrder_MstUserActive_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadWarehouseManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrhCancelOrder_MstUserActive_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhCancelOrder_MstUserActive_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");

            migrationBuilder.DropIndex(
                name: "IX_WrhCancelOrder_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");

            migrationBuilder.DropIndex(
                name: "IX_WrhCancelOrder_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");

            migrationBuilder.DropColumn(
                name: "HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");

            migrationBuilder.DropColumn(
                name: "HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder");
        }
    }
}
