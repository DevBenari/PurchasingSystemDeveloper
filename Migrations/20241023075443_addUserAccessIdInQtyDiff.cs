using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addUserAccessIdInQtyDiff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "UserAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_AspNetUsers_UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "UserAccessId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_AspNetUsers_UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "UserAccessId",
                schema: "dbo",
                table: "WrhQtyDifference");
        }
    }
}
