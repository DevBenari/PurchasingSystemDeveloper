using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class renameProductReturnDetaiInTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdProductReturnDetail_WrhProductReturn_ProductReturnId",
                schema: "dbo",
                table: "OrdProductReturnDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdProductReturnDetail",
                schema: "dbo",
                table: "OrdProductReturnDetail");

            migrationBuilder.RenameTable(
                name: "OrdProductReturnDetail",
                schema: "dbo",
                newName: "WrhProductReturnDetail",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_OrdProductReturnDetail_ProductReturnId",
                schema: "dbo",
                table: "WrhProductReturnDetail",
                newName: "IX_WrhProductReturnDetail_ProductReturnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WrhProductReturnDetail",
                schema: "dbo",
                table: "WrhProductReturnDetail",
                column: "ProductReturnDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_WrhProductReturnDetail_WrhProductReturn_ProductReturnId",
                schema: "dbo",
                table: "WrhProductReturnDetail",
                column: "ProductReturnId",
                principalSchema: "dbo",
                principalTable: "WrhProductReturn",
                principalColumn: "ProductReturnId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrhProductReturnDetail_WrhProductReturn_ProductReturnId",
                schema: "dbo",
                table: "WrhProductReturnDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WrhProductReturnDetail",
                schema: "dbo",
                table: "WrhProductReturnDetail");

            migrationBuilder.RenameTable(
                name: "WrhProductReturnDetail",
                schema: "dbo",
                newName: "OrdProductReturnDetail",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_WrhProductReturnDetail_ProductReturnId",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                newName: "IX_OrdProductReturnDetail_ProductReturnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdProductReturnDetail",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                column: "ProductReturnDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdProductReturnDetail_WrhProductReturn_ProductReturnId",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                column: "ProductReturnId",
                principalSchema: "dbo",
                principalTable: "WrhProductReturn",
                principalColumn: "ProductReturnId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
