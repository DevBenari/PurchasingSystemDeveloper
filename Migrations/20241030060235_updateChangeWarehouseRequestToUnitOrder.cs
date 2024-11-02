using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class updateChangeWarehouseRequestToUnitOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_WarehouseApprovalId",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhWarehouseTransfer_WrhWarehouseRequest_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.DropTable(
                name: "WrhApprovalRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhWarehouseRequestDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhWarehouseRequest",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_WrhWarehouseTransfer_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.RenameColumn(
                name: "WarehouseRequestNumber",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "UnitOrderNumber");

            migrationBuilder.RenameColumn(
                name: "WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "UserApprove1Id");

            migrationBuilder.RenameColumn(
                name: "UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "UnitOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_WrhWarehouseTransfer_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "IX_WrhWarehouseTransfer_UserApprove1Id");

            migrationBuilder.RenameColumn(
                name: "WarehouseApprovalId",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "UserApprove1Id");

            migrationBuilder.RenameColumn(
                name: "UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "Position1Id");

            migrationBuilder.RenameIndex(
                name: "IX_TscUnitRequest_WarehouseApprovalId",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "IX_TscUnitRequest_UserApprove1Id");

            migrationBuilder.RenameIndex(
                name: "IX_TscUnitRequest_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "IX_TscUnitRequest_Position1Id");

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Department1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageApprove1",
                schema: "dbo",
                table: "TscUnitRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WrhApprovalUnitRequest",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalUnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserApproveId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ApprovalStatusUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhApprovalUnitRequest", x => x.ApprovalUnitRequestId);
                    table.ForeignKey(
                        name: "FK_WrhApprovalUnitRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalUnitRequest_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalUnitRequest_MstUserActive_UserApproveId",
                        column: x => x.UserApproveId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalUnitRequest_MstWarehouseLocation_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstWarehouseLocation",
                        principalColumn: "WarehouseLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalUnitRequest_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhUnitOrder",
                schema: "dbo",
                columns: table => new
                {
                    UnitOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserApprove1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveStatusUser1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyTotal = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhUnitOrder", x => x.UnitOrderId);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrder_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrder_MstUnitLocation_UnitLocationId",
                        column: x => x.UnitLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstUnitLocation",
                        principalColumn: "UnitLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrder_MstUserActive_UserApprove1Id",
                        column: x => x.UserApprove1Id,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrder_MstWarehouseLocation_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalSchema: "dbo",
                        principalTable: "MstWarehouseLocation",
                        principalColumn: "WarehouseLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrder_TscUnitRequest_UnitRequestId",
                        column: x => x.UnitRequestId,
                        principalSchema: "dbo",
                        principalTable: "TscUnitRequest",
                        principalColumn: "UnitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhUnitOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    UnitOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtySent = table.Column<int>(type: "int", nullable: false),
                    Checked = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhUnitOrderDetail", x => x.UnitOrderDetailId);
                    table.ForeignKey(
                        name: "FK_WrhUnitOrderDetail_WrhUnitOrder_UnitOrderId",
                        column: x => x.UnitOrderId,
                        principalSchema: "dbo",
                        principalTable: "WrhUnitOrder",
                        principalColumn: "UnitOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TscUnitRequest_Department1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "Department1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalUnitRequest_UnitLocationId",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalUnitRequest_UnitRequestId",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalUnitRequest_UserAccessId",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalUnitRequest_UserApproveId",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                column: "UserApproveId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalUnitRequest_WarehouseLocationId",
                schema: "dbo",
                table: "WrhApprovalUnitRequest",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrder_UnitLocationId",
                schema: "dbo",
                table: "WrhUnitOrder",
                column: "UnitLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrder_UnitRequestId",
                schema: "dbo",
                table: "WrhUnitOrder",
                column: "UnitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrder_UserAccessId",
                schema: "dbo",
                table: "WrhUnitOrder",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrder_UserApprove1Id",
                schema: "dbo",
                table: "WrhUnitOrder",
                column: "UserApprove1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrder_WarehouseLocationId",
                schema: "dbo",
                table: "WrhUnitOrder",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhUnitOrderDetail_UnitOrderId",
                schema: "dbo",
                table: "WrhUnitOrderDetail",
                column: "UnitOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstDepartment_Department1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "Department1Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstPosition_Position1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "Position1Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhWarehouseTransfer_WrhUnitOrder_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseRequestId",
                principalSchema: "dbo",
                principalTable: "WrhUnitOrder",
                principalColumn: "UnitOrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstDepartment_Department1Id",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstPosition_Position1Id",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhWarehouseTransfer_WrhUnitOrder_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseTransfer");

            migrationBuilder.DropTable(
                name: "WrhApprovalUnitRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhUnitOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhUnitOrder",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_TscUnitRequest_Department1Id",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "Department1Id",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.DropColumn(
                name: "MessageApprove1",
                schema: "dbo",
                table: "TscUnitRequest");

            migrationBuilder.RenameColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "WarehouseApprovalId");

            migrationBuilder.RenameColumn(
                name: "UnitOrderNumber",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "WarehouseRequestNumber");

            migrationBuilder.RenameColumn(
                name: "UnitOrderId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "UnitRequestManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_WrhWarehouseTransfer_UserApprove1Id",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                newName: "IX_WrhWarehouseTransfer_WarehouseApprovalId");

            migrationBuilder.RenameColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "WarehouseApprovalId");

            migrationBuilder.RenameColumn(
                name: "Position1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "UnitRequestManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_TscUnitRequest_UserApprove1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "IX_TscUnitRequest_WarehouseApprovalId");

            migrationBuilder.RenameIndex(
                name: "IX_TscUnitRequest_Position1Id",
                schema: "dbo",
                table: "TscUnitRequest",
                newName: "IX_TscUnitRequest_UnitRequestManagerId");

            migrationBuilder.CreateTable(
                name: "WrhApprovalRequest",
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
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WarehouseApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WrhWarehouseRequest",
                schema: "dbo",
                columns: table => new
                {
                    WarehouseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitRequestManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarehouseApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    QtyTotal = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WarehouseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Checked = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtySent = table.Column<int>(type: "int", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                name: "IX_WrhWarehouseTransfer_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UnitRequestManagerId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "UnitRequestManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TscUnitRequest_MstUserActive_WarehouseApprovalId",
                schema: "dbo",
                table: "TscUnitRequest",
                column: "WarehouseApprovalId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_UnitRequestManagerId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "UnitRequestManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhWarehouseTransfer_MstUserActive_WarehouseApprovalId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseApprovalId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhWarehouseTransfer_WrhWarehouseRequest_WarehouseRequestId",
                schema: "dbo",
                table: "WrhWarehouseTransfer",
                column: "WarehouseRequestId",
                principalSchema: "dbo",
                principalTable: "WrhWarehouseRequest",
                principalColumn: "WarehouseRequestId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
