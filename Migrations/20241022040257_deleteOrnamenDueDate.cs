using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class deleteOrnamenDueDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApproval_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropTable(
                name: "MstDueDate",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_OrdApproval_DueDateId",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdApproval");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MstDueDate",
                schema: "dbo",
                columns: table => new
                {
                    DueDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstDueDate", x => x.DueDateId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                column: "DueDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApproval_MstDueDate_DueDateId",
                schema: "dbo",
                table: "OrdApproval",
                column: "DueDateId",
                principalSchema: "dbo",
                principalTable: "MstDueDate",
                principalColumn: "DueDateId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
