using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateWarehouseLocationTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropIndex(
                name: "IX_MstWarehouseLocation_OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropColumn(
                name: "OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.RenameColumn(
                name: "WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                newName: "UserActiveId");

            migrationBuilder.RenameIndex(
                name: "IX_MstWarehouseLocation_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                newName: "IX_MstWarehouseLocation_UserActiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "UserActiveId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.RenameColumn(
                name: "UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                newName: "WarehouseManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_MstWarehouseLocation_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                newName: "IX_MstWarehouseLocation_WarehouseManagerId");

            migrationBuilder.AddColumn<Guid>(
                name: "OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstWarehouseLocation_OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "OperationalChiefId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "OperationalChiefId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

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
    }
}
