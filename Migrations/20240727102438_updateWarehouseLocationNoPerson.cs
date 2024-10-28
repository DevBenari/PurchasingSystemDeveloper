using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateWarehouseLocationNoPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstWarehouseLocation_MstUserActive_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropIndex(
                name: "IX_MstWarehouseLocation_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation");

            migrationBuilder.DropColumn(
                name: "UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstWarehouseLocation_UserActiveId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "UserActiveId");

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
    }
}
