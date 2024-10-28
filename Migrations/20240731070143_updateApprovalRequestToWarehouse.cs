using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateApprovalRequestToWarehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TscApprovalRequest",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "WrhApprovalRequest",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_WrhApprovalRequest", x => x.ApprovalRequestId);
                    table.ForeignKey(
                        name: "FK_WrhApprovalRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalRequest_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalRequest_MstUserActive_UnitRequestManagerId",
                        column: x => x.UnitRequestManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalRequest_MstUserActive_WarehouseApprovalId",
                        column: x => x.WarehouseApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalRequest_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalRequest_UnitLocationId",
                schema: "dbo",
                table: "WrhApprovalRequest",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalRequest_UnitRequestId",
                schema: "dbo",
                table: "WrhApprovalRequest",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhApprovalRequest",
                column: "UnitRequestManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalRequest_UserAccessId",
                schema: "dbo",
                table: "WrhApprovalRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalRequest_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhApprovalRequest",
                column: "WarehouseApprovalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrhApprovalRequest",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "TscApprovalRequest",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarehouseApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TscApprovalRequest", x => x.ApprovalRequestId);
                    table.ForeignKey(
                        name: "FK_TscApprovalRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TscApprovalRequest_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TscApprovalRequest_MstUserActive_UnitRequestManagerId",
                        column: x => x.UnitRequestManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TscApprovalRequest_MstUserActive_WarehouseApprovalId",
                        column: x => x.WarehouseApprovalId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TscApprovalRequest_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TscApprovalRequest_UnitLocationId",
                schema: "dbo",
                table: "TscApprovalRequest",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TscApprovalRequest_UnitRequestId",
                schema: "dbo",
                table: "TscApprovalRequest",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TscApprovalRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "TscApprovalRequest",
                column: "UnitRequestManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TscApprovalRequest_UserAccessId",
                schema: "dbo",
                table: "TscApprovalRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_TscApprovalRequest_WarehouseApprovalId",
                schema: "dbo",
                table: "TscApprovalRequest",
                column: "WarehouseApprovalId");
        }
    }
}
