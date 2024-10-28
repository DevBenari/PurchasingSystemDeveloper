using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addIsPKSIsActiveInSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "MstSupplier",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPKS",
                schema: "dbo",
                table: "MstSupplier",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LeadTimeId",
                schema: "dbo",
                table: "MstSupplier",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstSupplier_LeadTimeId",
                schema: "dbo",
                table: "MstSupplier",
                column: "LeadTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstSupplier_MstLeadTime_LeadTimeId",
                schema: "dbo",
                table: "MstSupplier",
                column: "LeadTimeId",
                principalSchema: "dbo",
                principalTable: "MstLeadTime",
                principalColumn: "LeadTimeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstSupplier_MstLeadTime_LeadTimeId",
                schema: "dbo",
                table: "MstSupplier");

            migrationBuilder.DropIndex(
                name: "IX_MstSupplier_LeadTimeId",
                schema: "dbo",
                table: "MstSupplier");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "MstSupplier");

            migrationBuilder.DropColumn(
                name: "IsPKS",
                schema: "dbo",
                table: "MstSupplier");

            migrationBuilder.DropColumn(
                name: "LeadTimeId",
                schema: "dbo",
                table: "MstSupplier");
        }
    }
}
