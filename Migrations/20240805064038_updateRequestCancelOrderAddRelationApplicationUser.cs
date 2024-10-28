using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateRequestCancelOrderAddRelationApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
