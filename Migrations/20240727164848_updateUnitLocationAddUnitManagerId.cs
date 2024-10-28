using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateUnitLocationAddUnitManagerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstUnitLocation_UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation",
                column: "UnitManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstUnitLocation_MstUserActive_UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation",
                column: "UnitManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstUnitLocation_MstUserActive_UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation");

            migrationBuilder.DropIndex(
                name: "IX_MstUnitLocation_UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation");

            migrationBuilder.DropColumn(
                name: "UnitManagerId",
                schema: "dbo",
                table: "MstUnitLocation");
        }
    }
}
