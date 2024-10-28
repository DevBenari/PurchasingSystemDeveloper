using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addColumnQtyDifferenceDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Measure",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                newName: "Supplier");

            migrationBuilder.AddColumn<int>(
                name: "Discount",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Measurement",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail");

            migrationBuilder.DropColumn(
                name: "Measurement",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                newName: "Measure");
        }
    }
}
