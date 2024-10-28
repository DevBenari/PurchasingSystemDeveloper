using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateUserActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MstUser",
                schema: "dbo",
                table: "MstUser");

            migrationBuilder.RenameTable(
                name: "MstUser",
                schema: "dbo",
                newName: "MstUserActive",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "UserCode",
                schema: "dbo",
                table: "MstUserActive",
                newName: "UserActiveCode");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "MstUserActive",
                newName: "UserActiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MstUserActive",
                schema: "dbo",
                table: "MstUserActive",
                column: "UserActiveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MstUserActive",
                schema: "dbo",
                table: "MstUserActive");

            migrationBuilder.RenameTable(
                name: "MstUserActive",
                schema: "dbo",
                newName: "MstUser",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "UserActiveCode",
                schema: "dbo",
                table: "MstUser",
                newName: "UserCode");

            migrationBuilder.RenameColumn(
                name: "UserActiveId",
                schema: "dbo",
                table: "MstUser",
                newName: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MstUser",
                schema: "dbo",
                table: "MstUser",
                column: "UserId");
        }
    }
}
