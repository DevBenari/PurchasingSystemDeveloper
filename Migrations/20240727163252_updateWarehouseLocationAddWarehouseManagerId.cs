using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateWarehouseLocationAddWarehouseManagerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseManager",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstWarehouseLocation_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "WarehouseManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "WarehouseManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropIndex(
                name: "IX_MstWarehouseLocation_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropColumn(
                name: "WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseManager",
                schema: "dbo",
                table: "MstWarehouseLocation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
