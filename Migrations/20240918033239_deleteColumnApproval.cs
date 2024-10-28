using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class deleteColumnApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.RenameColumn(
                name: "ApproveStatusUser3",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveStatusUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApproveStatusUser",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ApproveStatusUser3");

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "OrdApproval",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
