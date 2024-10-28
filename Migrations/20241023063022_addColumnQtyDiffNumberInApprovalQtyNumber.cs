using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addColumnQtyDiffNumberInApprovalQtyNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QtyDifferenceNumber",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtyDifferenceNumber",
                schema: "dbo",
                table: "OrdApprovalQtyDifference");
        }
    }
}
