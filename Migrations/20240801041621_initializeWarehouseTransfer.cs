using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeWarehouseTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WrhWarehouseTransfer",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseTransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseTransferNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarehouseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyTotal = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_WrhWarehouseTransfer", x => x.WarehouseTransferId);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_MstUserActive_UnitRequestManagerId",
                        column: x => x.UnitRequestManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_MstUserActive_WarehouseApprovalId",
                        column: x => x.WarehouseApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_MstWarehouseLocation_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstWarehouseLocation",
                        principalColumn: "WarehouseLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransfer_WrhWarehouseRequest_WarehouseRequestId",
                        column: x => x.WarehouseRequestId,
                        principalSchema: "dbo",
                        principalTable: "WrhWarehouseRequest",
                        principalColumn: "WarehouseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhWarehouseTransferDetail",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseTransferDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseTransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Principal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtySent = table.Column<int>(type: "int", nullable: false),
                    Checked = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_WrhWarehouseTransferDetail", x => x.WarehouseTransferDetailId);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseTransferDetail_WrhWarehouseTransfer_WarehouseTransferId",
                        column: x => x.WarehouseTransferId,
                        principalSchema: "dbo",
                        principalTable: "WrhWarehouseTransfer",
                        principalColumn: "WarehouseTransferId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_UnitLocationId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UnitRequestManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_UserAccessId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_WarehouseLocationId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransfer_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseTransferDetail_WarehouseTransferId",
                schema: "dbo",
                table: "WrhWarehouseTransferDetail",
                column: "WarehouseTransferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrhWarehouseTransferDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhWarehouseTransfer",
                schema: "dbo");
        }
    }
}
