﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class addColumnQtyTotalInApprovalUnitRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtyTotal",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtyTotal",
                schema: "dbo",
                table: "WrhApprovalUnitRequest");
        }
    }
}
