using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeWarehouseLocationAndAddInProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RackNumber",
                schema: "dbo",
                table: "MstProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocation",
                schema: "dbo",
                table: "MstProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MstWarehouseLocation",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseLocationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarehouseLocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationalChiefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstWarehouseLocation", x => x.WarehouseLocationId);
                    table.ForeignKey(
                        name: "FK_MstWarehouseLocation_MstUserActive_OperationalChiefId",
                        column: x => x.OperationalChiefId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstWarehouseLocation_MstUserActive_WarehouseManagerId",
                        column: x => x.WarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MstProduct_WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MstWarehouseLocation_OperationalChiefId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "OperationalChiefId");

            migrationBuilder.CreateIndex(
                name: "IX_MstWarehouseLocation_WarehouseManagerId",
                schema: "dbo",
                table: "MstWarehouseLocation",
                column: "WarehouseManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MstProduct_MstWarehouseLocation_WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct",
                column: "WarehouseLocationId",
                principalSchema: "dbo",
                principalTable: "MstWarehouseLocation",
                principalColumn: "WarehouseLocationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstProduct_MstWarehouseLocation_WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropTable(
                name: "MstWarehouseLocation",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_MstProduct_WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropColumn(
                name: "RackNumber",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropColumn(
                name: "StorageLocation",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseLocationId",
                schema: "dbo",
                table: "MstProduct");
        }
    }
}
