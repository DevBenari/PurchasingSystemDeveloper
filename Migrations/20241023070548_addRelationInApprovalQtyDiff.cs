using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addRelationInApprovalQtyDiff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.AddColumn<string>(
                name: "UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalQtyDifference_UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                column: "UserAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdApprovalQtyDifference_AspNetUsers_UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                column: "UserAccessId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdApprovalQtyDifference_AspNetUsers_UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_OrdApprovalQtyDifference_UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference");

            migrationBuilder.DropColumn(
                name: "UserAccessId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference");           
        }
    }
}
