using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addRemainingDayRenameExpiredOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiredOrder",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                newName: "RemainingDay");

            migrationBuilder.RenameColumn(
                name: "ExpiredOrder",
                schema: "dbo",
                table: "OrdApproval",
                newName: "RemainingDay");

            migrationBuilder.AddColumn<int>(
                name: "ExpiredDay",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpiredDay",
                schema: "dbo",
                table: "OrdApproval",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredDay",
                schema: "dbo",
                table: "OrdPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "ExpiredDay",
                schema: "dbo",
                table: "OrdApproval");

            migrationBuilder.RenameColumn(
                name: "RemainingDay",
                schema: "dbo",
                table: "OrdPurchaseRequest",
                newName: "ExpiredOrder");

            migrationBuilder.RenameColumn(
                name: "RemainingDay",
                schema: "dbo",
                table: "OrdApproval",
                newName: "ExpiredOrder");
        }
    }
}
