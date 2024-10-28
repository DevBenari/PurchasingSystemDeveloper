using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class changeColumnInEmailSupplier2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                schema: "dbo",
                table: "OrdPurchaseEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
