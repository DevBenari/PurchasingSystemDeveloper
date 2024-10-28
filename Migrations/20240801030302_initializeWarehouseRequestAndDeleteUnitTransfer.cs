using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeWarehouseRequestAndDeleteUnitTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrhUnitTransferDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhUnitTransfer",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "WrhWarehouseRequest",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_WrhWarehouseRequest", x => x.WarehouseRequestId);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_MstUserActive_UnitRequestManagerId",
                        column: x => x.UnitRequestManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_MstUserActive_WarehouseApprovalId",
                        column: x => x.WarehouseApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_MstWarehouseLocation_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstWarehouseLocation",
                        principalColumn: "WarehouseLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequest_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhWarehouseRequestDetail",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseRequestDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_WrhWarehouseRequestDetail", x => x.WarehouseRequestDetailId);
                    table.ForeignKey(
                        name: "FK_WrhWarehouseRequestDetail_WrhWarehouseRequest_WarehouseRequestId",
                        column: x => x.WarehouseRequestId,
                        principalSchema: "dbo",
                        principalTable: "WrhWarehouseRequest",
                        principalColumn: "WarehouseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_UnitLocationId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_UnitRequestId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "UnitRequestManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_UserAccessId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "WarehouseApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequest_WarehouseLocationId",
                schema: "dbo",
                table: "WrhWarehouseRequest",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhWarehouseRequestDetail_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseRequestDetail",
                column: "WarehouseRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrhWarehouseRequestDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhWarehouseRequest",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "WrhUnitTransfer",
                schema: "dbo",
                columns: table => new
                {
                    UnitTransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarehouseApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    QtyTotal = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitTransferNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhUnitTransfer", x => x.UnitTransferId);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_MstUserActive_UnitRequestManagerId",
                        column: x => x.UnitRequestManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_MstUserActive_WarehouseApprovalId",
                        column: x => x.WarehouseApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_MstWarehouseLocation_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstWarehouseLocation",
                        principalColumn: "WarehouseLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransfer_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhUnitTransferDetail",
                schema: "dbo",
                columns: table => new
                {
                    UnitTransferDetaillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Checked = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Principal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtySent = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhUnitTransferDetail", x => x.UnitTransferDetaillId);
                    table.ForeignKey(
                        name: "FK_WrhUnitTransferDetail_WrhUnitTransfer_UnitTransferId",
                        column: x => x.UnitTransferId,
                        principalSchema: "dbo",
                        principalTable: "WrhUnitTransfer",
                        principalColumn: "UnitTransferId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_UnitLocationId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_UnitRequestId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "UnitRequestManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_UserAccessId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "WarehouseApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransfer_WarehouseLocationId",
                schema: "dbo",
                table: "WrhUnitTransfer",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitTransferDetail_UnitTransferId",
                schema: "dbo",
                table: "WrhUnitTransferDetail",
                column: "UnitTransferId");
        }
    }
}
