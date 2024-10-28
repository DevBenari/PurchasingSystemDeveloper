using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeInitialStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MstInitialStock",
                schema: "dbo",
                columns: table => new
                {
                    InitialStockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrincipalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeadTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaxRequest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageRequest = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_MstInitialStock", x => x.InitialStockId);
                    table.ForeignKey(
                        name: "FK_MstInitialStock_MstLeadTime_LeadTimeId",
                        column: x => x.LeadTimeId,
                        principalSchema: "dbo",
                        principalTable: "MstLeadTime",
                        principalColumn: "LeadTimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstInitialStock_MstPrincipal_PrincipalId",
                        column: x => x.PrincipalId,
                        principalSchema: "dbo",
                        principalTable: "MstPrincipal",
                        principalColumn: "PrincipalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MstInitialStock_MstProduct_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "MstProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MstInitialStock_LeadTimeId",
                schema: "dbo",
                table: "MstInitialStock",
                column: "LeadTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_MstInitialStock_PrincipalId",
                schema: "dbo",
                table: "MstInitialStock",
                column: "PrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_MstInitialStock_ProductId",
                schema: "dbo",
                table: "MstInitialStock",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MstInitialStock",
                schema: "dbo");
        }
    }
}
