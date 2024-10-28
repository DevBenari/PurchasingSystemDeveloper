using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class changeColumnInEmailSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Emailid",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                newName: "EmailId");

            migrationBuilder.RenameColumn(
                name: "Pesan",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                newName: "Message");

            migrationBuilder.AddColumn<string>(
                name: "Document",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                schema: "dbo",
                table: "OrdPurchaseEmail");

            migrationBuilder.RenameColumn(
                name: "EmailId",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                newName: "Emailid");

            migrationBuilder.RenameColumn(
                name: "Message",
                schema: "dbo",
                table: "OrdPurchaseEmail",
                newName: "Pesan");
        }
    }
}
