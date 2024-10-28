using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateColumnApprovalDateAndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApproveTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApprovalTime");

            migrationBuilder.RenameColumn(
                name: "ApproveStatusUser",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApprovalStatusUser");

            migrationBuilder.RenameColumn(
                name: "ApproveDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApprovalDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovalTime",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveTime");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatusUser",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveStatusUser");

            migrationBuilder.RenameColumn(
                name: "ApprovalDate",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveDate");
        }
    }
}
