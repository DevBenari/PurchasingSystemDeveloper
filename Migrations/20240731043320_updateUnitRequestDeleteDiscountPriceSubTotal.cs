using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateUnitRequestDeleteDiscountPriceSubTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                schema: "dbo",
                table: "TscUnitRequestDetail");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "dbo",
                table: "TscUnitRequestDetail");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "TscUnitRequestDetail");

            migrationBuilder.DropColumn(
                name: "GrandTotal",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "QtyTotal",
                schema: "dbo",
                table: "TscUnitRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Discount",
                schema: "dbo",
                table: "TscUnitRequestDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "dbo",
                table: "TscUnitRequestDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                schema: "dbo",
                table: "TscUnitRequestDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "QtyTotal",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
