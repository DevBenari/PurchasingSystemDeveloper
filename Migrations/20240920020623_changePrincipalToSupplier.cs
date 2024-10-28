using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class changePrincipalToSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstInitialStock_MstPrincipal_PrincipalId",
                schema: "dbo",
                table: "MstInitialStock");

            migrationBuilder.DropForeignKey(
                name: "FK_MstProduct_MstPrincipal_PrincipalId",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropTable(
                name: "MstPrincipal",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "Principal",
                schema: "dbo",
                table: "WrhWarehouseTransferDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "Principal",
                schema: "dbo",
                table: "WrhWarehouseRequestDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "Principal",
                schema: "dbo",
                table: "TscUnitRequestDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "Principal",
                schema: "dbo",
                table: "OrdPurchaseRequestDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "Principal",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "PrincipalId",
                schema: "dbo",
                table: "MstProduct",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MstProduct_PrincipalId",
                schema: "dbo",
                table: "MstProduct",
                newName: "IX_MstProduct_SupplierId");

            migrationBuilder.RenameColumn(
                name: "PrincipalName",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "SupplierName");

            migrationBuilder.RenameColumn(
                name: "PrincipalId",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MstInitialStock_PrincipalId",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "IX_MstInitialStock_SupplierId");

            migrationBuilder.CreateTable(
                name: "MstSupplier",
                schema: "dbo",
                columns: table => new
                {
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Handphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_MstSupplier", x => x.SupplierId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MstInitialStock_MstSupplier_SupplierId",
                schema: "dbo",
                table: "MstInitialStock",
                column: "SupplierId",
                principalSchema: "dbo",
                principalTable: "MstSupplier",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MstProduct_MstSupplier_SupplierId",
                schema: "dbo",
                table: "MstProduct",
                column: "SupplierId",
                principalSchema: "dbo",
                principalTable: "MstSupplier",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MstInitialStock_MstSupplier_SupplierId",
                schema: "dbo",
                table: "MstInitialStock");

            migrationBuilder.DropForeignKey(
                name: "FK_MstProduct_MstSupplier_SupplierId",
                schema: "dbo",
                table: "MstProduct");

            migrationBuilder.DropTable(
                name: "MstSupplier",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "WrhWarehouseTransferDetail",
                newName: "Principal");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "WrhWarehouseRequestDetail",
                newName: "Principal");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "TscUnitRequestDetail",
                newName: "Principal");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "OrdPurchaseRequestDetail",
                newName: "Principal");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "Principal");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                schema: "dbo",
                table: "MstProduct",
                newName: "PrincipalId");

            migrationBuilder.RenameIndex(
                name: "IX_MstProduct_SupplierId",
                schema: "dbo",
                table: "MstProduct",
                newName: "IX_MstProduct_PrincipalId");

            migrationBuilder.RenameColumn(
                name: "SupplierName",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "PrincipalName");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "PrincipalId");

            migrationBuilder.RenameIndex(
                name: "IX_MstInitialStock_SupplierId",
                schema: "dbo",
                table: "MstInitialStock",
                newName: "IX_MstInitialStock_PrincipalId");

            migrationBuilder.CreateTable(
                name: "MstPrincipal",
                schema: "dbo",
                columns: table => new
                {
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Handphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrincipalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstPrincipal", x => x.PrincipalId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MstInitialStock_MstPrincipal_PrincipalId",
                schema: "dbo",
                table: "MstInitialStock",
                column: "PrincipalId",
                principalSchema: "dbo",
                principalTable: "MstPrincipal",
                principalColumn: "PrincipalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MstProduct_MstPrincipal_PrincipalId",
                schema: "dbo",
                table: "MstProduct",
                column: "PrincipalId",
                principalSchema: "dbo",
                principalTable: "MstPrincipal",
                principalColumn: "PrincipalId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
