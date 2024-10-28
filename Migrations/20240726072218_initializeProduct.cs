using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MstProduct",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MeasurementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinStock = table.Column<int>(type: "int", nullable: true),
                    MaxStock = table.Column<int>(type: "int", nullable: true),
                    BufferStock = table.Column<int>(type: "int", nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: true),
                    Cogs = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BuyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_MstProduct", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_MstProduct_MstCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "MstCategory",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstProduct_MstDiscount_DiscountId",
                        column: x => x.DiscountId,
                        principalSchema: "dbo",
                        principalTable: "MstDiscount",
                        principalColumn: "DiscountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstProduct_MstMeasurement_MeasurementId",
                        column: x => x.MeasurementId,
                        principalSchema: "dbo",
                        principalTable: "MstMeasurement",
                        principalColumn: "MeasurementId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstProduct_MstPrincipal_PrincipalId",
                        column: x => x.PrincipalId,
                        principalSchema: "dbo",
                        principalTable: "MstPrincipal",
                        principalColumn: "PrincipalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MstProduct_CategoryId",
                schema: "dbo",
                table: "MstProduct",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MstProduct_DiscountId",
                schema: "dbo",
                table: "MstProduct",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_MstProduct_MeasurementId",
                schema: "dbo",
                table: "MstProduct",
                column: "MeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_MstProduct_PrincipalId",
                schema: "dbo",
                table: "MstProduct",
                column: "PrincipalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MstProduct",
                schema: "dbo");
        }
    }
}
