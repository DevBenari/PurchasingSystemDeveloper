using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateUserActiveAddDepartmentAndPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "dbo",
                table: "MstUserActive",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId",
                schema: "dbo",
                table: "MstUserActive",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstUserActive_DepartmentId",
                schema: "dbo",
                table: "MstUserActive",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MstUserActive_PositionId",
                schema: "dbo",
                table: "MstUserActive",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstUserActive_MstDepartment_DepartmentId",
                schema: "dbo",
                table: "MstUserActive",
                column: "DepartmentId",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MstUserActive_MstPosition_PositionId",
                schema: "dbo",
                table: "MstUserActive",
                column: "PositionId",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstUserActive_MstDepartment_DepartmentId",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.DropForeignKey(
                name: "FK_MstUserActive_MstPosition_PositionId",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.DropIndex(
                name: "IX_MstUserActive_DepartmentId",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.DropIndex(
                name: "IX_MstUserActive_PositionId",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.DropColumn(
                name: "PositionId",
                schema: "dbo",
                table: "MstUserActive");
        }
    }
}
