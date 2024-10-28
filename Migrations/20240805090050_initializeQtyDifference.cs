using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeQtyDifference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhCancelOrder_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "OrdRequestCancelOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhCancelOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhCancelOrder",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "QtyDifferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseOrderDetail_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "IX_OrdPurchaseOrderDetail_QtyDifferenceId");

            migrationBuilder.CreateTable(
                name: "WrhQtyDifference",
                schema: "dbo",
                columns: table => new
                {
                    QtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyDifferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_WrhQtyDifference", x => x.QtyDifferenceId);
                    table.ForeignKey(
                        name: "FK_WrhQtyDifference_AspNetUsers_CheckedById",
                        column: x => x.CheckedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhQtyDifference_MstUserActive_HeadPurchasingManagerId",
                        column: x => x.HeadPurchasingManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhQtyDifference_MstUserActive_HeadWarehouseManagerId",
                        column: x => x.HeadWarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhQtyDifference_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdQtyDifferenceRequest",
                schema: "dbo",
                columns: table => new
                {
                    QtyDifferenceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QtyDifferenceApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QtyDifferenceApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_OrdQtyDifferenceRequest", x => x.QtyDifferenceRequestId);
                    table.ForeignKey(
                        name: "FK_OrdQtyDifferenceRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdQtyDifferenceRequest_MstUserActive_HeadPurchasingManagerId",
                        column: x => x.HeadPurchasingManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdQtyDifferenceRequest_MstUserActive_HeadWarehouseManagerId",
                        column: x => x.HeadWarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdQtyDifferenceRequest_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdQtyDifferenceRequest_WrhQtyDifference_QtyDifferenceId",
                        column: x => x.QtyDifferenceId,
                        principalSchema: "dbo",
                        principalTable: "WrhQtyDifference",
                        principalColumn: "QtyDifferenceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhQtyDifferenceDetail",
                schema: "dbo",
                columns: table => new
                {
                    QtyDifferenceDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyOrder = table.Column<int>(type: "int", nullable: false),
                    QtyReceive = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_WrhQtyDifferenceDetail", x => x.QtyDifferenceDetailId);
                    table.ForeignKey(
                        name: "FK_WrhQtyDifferenceDetail_WrhQtyDifference_QtyDifferenceId",
                        column: x => x.QtyDifferenceId,
                        principalSchema: "dbo",
                        principalTable: "WrhQtyDifference",
                        principalColumn: "QtyDifferenceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdQtyDifferenceRequest_HeadPurchasingManagerId",
                schema: "dbo",
                table: "OrdQtyDifferenceRequest",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdQtyDifferenceRequest_HeadWarehouseManagerId",
                schema: "dbo",
                table: "OrdQtyDifferenceRequest",
                column: "HeadWarehouseManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdQtyDifferenceRequest_PurchaseOrderId",
                schema: "dbo",
                table: "OrdQtyDifferenceRequest",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdQtyDifferenceRequest_QtyDifferenceId",
                schema: "dbo",
                table: "OrdQtyDifferenceRequest",
                column: "QtyDifferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdQtyDifferenceRequest_UserAccessId",
                schema: "dbo",
                table: "OrdQtyDifferenceRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_CheckedById",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "CheckedById");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "HeadWarehouseManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifference_PurchaseOrderId",
                schema: "dbo",
                table: "WrhQtyDifference",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhQtyDifferenceDetail_QtyDifferenceId",
                schema: "dbo",
                table: "WrhQtyDifferenceDetail",
                column: "QtyDifferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhQtyDifference_QtyDifferenceId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "QtyDifferenceId",
                principalSchema: "dbo",
                principalTable: "WrhQtyDifference",
                principalColumn: "QtyDifferenceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhQtyDifference_QtyDifferenceId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "OrdQtyDifferenceRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhQtyDifferenceDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhQtyDifference",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "QtyDifferenceId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "CancelOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdPurchaseOrderDetail_QtyDifferenceId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                newName: "IX_OrdPurchaseOrderDetail_CancelOrderId");

            migrationBuilder.CreateTable(
                name: "OrdRequestCancelOrder",
                schema: "dbo",
                columns: table => new
                {
                    RequestCancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestCancelApproveById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestCancelApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestCancelOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdRequestCancelOrder", x => x.RequestCancelOrderId);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_AspNetUsers_RequestCancelApproveById",
                        column: x => x.RequestCancelApproveById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_MstUserActive_HeadPurchasingManagerId",
                        column: x => x.HeadPurchasingManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_MstUserActive_HeadWarehouseManagerId",
                        column: x => x.HeadWarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdRequestCancelOrder_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhCancelOrder",
                schema: "dbo",
                columns: table => new
                {
                    CancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhCancelOrder", x => x.CancelOrderId);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_AspNetUsers_CancelById",
                        column: x => x.CancelById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_MstUserActive_HeadPurchasingManagerId",
                        column: x => x.HeadPurchasingManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_MstUserActive_HeadWarehouseManagerId",
                        column: x => x.HeadWarehouseManagerId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrder_OrdPurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseOrder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WrhCancelOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    CancelOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyCancel = table.Column<int>(type: "int", nullable: false),
                    QtyOrder = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrhCancelOrderDetail", x => x.CancelOrderDetailId);
                    table.ForeignKey(
                        name: "FK_WrhCancelOrderDetail_WrhCancelOrder_CancelOrderId",
                        column: x => x.CancelOrderId,
                        principalSchema: "dbo",
                        principalTable: "WrhCancelOrder",
                        principalColumn: "CancelOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_HeadPurchasingManagerId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_HeadWarehouseManagerId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "HeadWarehouseManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_PurchaseOrderId",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdRequestCancelOrder_RequestCancelApproveById",
                schema: "dbo",
                table: "OrdRequestCancelOrder",
                column: "RequestCancelApproveById");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_CancelById",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "CancelById");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_HeadPurchasingManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadPurchasingManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_HeadWarehouseManagerId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "HeadWarehouseManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrder_PurchaseOrderId",
                schema: "dbo",
                table: "WrhCancelOrder",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhCancelOrderDetail_CancelOrderId",
                schema: "dbo",
                table: "WrhCancelOrderDetail",
                column: "CancelOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdPurchaseOrderDetail_WrhCancelOrder_CancelOrderId",
                schema: "dbo",
                table: "OrdPurchaseOrderDetail",
                column: "CancelOrderId",
                principalSchema: "dbo",
                principalTable: "WrhCancelOrder",
                principalColumn: "CancelOrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
