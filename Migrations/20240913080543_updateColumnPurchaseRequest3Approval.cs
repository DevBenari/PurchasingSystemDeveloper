using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateColumnPurchaseRequest3Approval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department3Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdPurchaseRequest_Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position3Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department1Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department2Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Department3Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position1Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position2Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                column: "Position3Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstDepartment_Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseRequest_MstPosition_Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_OrdPurchaseRequest_Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Department1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Department2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Department3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Position1Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Position2Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Position3Id",
                schema: "dbo",
                table: "OrdPurchaseRequest");
        }
    }
}
