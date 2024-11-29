using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addColumnInReceiveOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                schema: "dbo",
                table: "WrhReceiveOrderDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryDate",
                schema: "dbo",
                table: "WrhReceiveOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryServiceName",
                schema: "dbo",
                table: "WrhReceiveOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "WrhReceiveOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingNumber",
                schema: "dbo",
                table: "WrhReceiveOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WaybillNumber",
                schema: "dbo",
                table: "WrhReceiveOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                schema: "dbo",
                table: "WrhReceiveOrderDetail");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                schema: "dbo",
                table: "WrhReceiveOrder");

            migrationBuilder.DropColumn(
                name: "DeliveryServiceName",
                schema: "dbo",
                table: "WrhReceiveOrder");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "WrhReceiveOrder");

            migrationBuilder.DropColumn(
                name: "ShippingNumber",
                schema: "dbo",
                table: "WrhReceiveOrder");

            migrationBuilder.DropColumn(
                name: "WaybillNumber",
                schema: "dbo",
                table: "WrhReceiveOrder");
        }
    }
}
