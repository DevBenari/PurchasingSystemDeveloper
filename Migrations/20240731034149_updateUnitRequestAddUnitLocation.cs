using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateUnitRequestAddUnitLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TscUnitRequest_UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TscUnitRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UnitRequestManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstUnitLocation_UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UnitLocationId",
                principalSchema: "dbo",
                principalTable: "MstUnitLocation",
                principalColumn: "UnitLocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UnitRequestManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstUnitLocation_UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropIndex(
                name: "IX_TscUnitRequest_UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropIndex(
                name: "IX_TscUnitRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "UnitLocationId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest");
        }
    }
}
