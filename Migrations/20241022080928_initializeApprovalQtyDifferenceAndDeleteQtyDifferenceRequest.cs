using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeApprovalQtyDifferenceAndDeleteQtyDifferenceRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_AspNetUsers_CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.RenameColumn(
                name: "HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "UserApprove2Id");

            migrationBuilder.RenameColumn(
                name: "HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "UserApprove1Id");

            migrationBuilder.RenameIndex(
                name: "IX_WrhQtyDifference_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "IX_WrhQtyDifference_UserApprove2Id");

            migrationBuilder.RenameIndex(
                name: "IX_WrhQtyDifference_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "IX_WrhQtyDifference_UserApprove1Id");

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageApprove1",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageApprove2",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrdApprovalQtyDifference",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalQtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_OrdApprovalQtyDifference", x => x.ApprovalQtyDifferenceId);
                    table.ForeignKey(
                        name: "FK_OrdApprovalQtyDifference_MstUserActive_UserApproveId",
                        column: x => x.UserApproveId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApprovalQtyDifference_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApprovalQtyDifference_WrhQtyDifference_QtyDifferenceId",
                        column: x => x.QtyDifferenceId,
                        principalSchema: "dbo",
                        principalTable: "WrhQtyDifference",
                        principalColumn: "QtyDifferenceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Department1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Department2Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Position1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Position2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalQtyDifference_PurchaseOrderId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalQtyDifference_QtyDifferenceId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                column: "QtyDifferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalQtyDifference_UserApproveId",
                schema: "dbo",
                table: "OrdApprovalQtyDifference",
                column: "UserApproveId");

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstDepartment_Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Department1Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstDepartment_Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Department2Id",
                principalSchema: "dbo",
                principalTable: "MstDepartment",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstPosition_Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Position1Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstPosition_Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "Position2Id",
                principalSchema: "dbo",
                principalTable: "MstPosition",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "UserApprove1Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "UserApprove2Id",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstDepartment_Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstDepartment_Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstPosition_Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstPosition_Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_UserApprove1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_UserApprove2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropTable(
                name: "OrdApprovalQtyDifference",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropIndex(
                name: "IX_WrhQtyDifference_Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser1",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "ApproveStatusUser2",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "Department1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "Department2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "MessageApprove1",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "MessageApprove2",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "Position1Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "Position2Id",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                schema: "dbo",
                table: "WrhQtyDifference");

            migrationBuilder.RenameColumn(
                name: "UserApprove2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "HeadWarehouseManagerId");

            migrationBuilder.RenameColumn(
                name: "UserApprove1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "HeadPurchasingManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_WrhQtyDifference_UserApprove2Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "IX_WrhQtyDifference_HeadWarehouseManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_WrhQtyDifference_UserApprove1Id",
                schema: "dbo",
                table: "WrhQtyDifference",
                newName: "IX_WrhQtyDifference_HeadPurchasingManagerId");

            migrationBuilder.AddColumn<string>(
                name: "CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "CheckedById");

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_AspNetUsers_CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "CheckedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "HeadPurchasingManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WrhQtyDifference_MstUserActive_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "HeadWarehouseManagerId",
                principalSchema: "dbo",
                principalTable: "MstUserActive",
                principalColumn: "UserActiveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
