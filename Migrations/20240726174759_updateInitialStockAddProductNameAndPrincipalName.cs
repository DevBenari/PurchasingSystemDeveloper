using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateInitialStockAddProductNameAndPrincipalName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrincipalName",
                schema: "dbo",
                table: "MstInitialStock",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                schema: "dbo",
                table: "MstInitialStock",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrincipalName",
                schema: "dbo",
                table: "MstInitialStock");

            migrationBuilder.DropColumn(
                name: "ProductName",
                schema: "dbo",
                table: "MstInitialStock");
        }
    }
}
