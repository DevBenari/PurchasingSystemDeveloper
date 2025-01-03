using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addColumnInProductReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseLocation",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                newName: "WarehouseOrigin");

            migrationBuilder.AddColumn<string>(
                name: "ReasonForReturn",
                schema: "dbo",
                table: "WrhProductReturn",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReturnDate",
                schema: "dbo",
                table: "WrhProductReturn",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiredDate",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "WarehouseExpired",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonForReturn",
                schema: "dbo",
                table: "WrhProductReturn");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                schema: "dbo",
                table: "WrhProductReturn");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                schema: "dbo",
                table: "OrdProductReturnDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseExpired",
                schema: "dbo",
                table: "OrdProductReturnDetail");

            migrationBuilder.RenameColumn(
                name: "WarehouseOrigin",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                newName: "WarehouseLocation");
        }
    }
}
