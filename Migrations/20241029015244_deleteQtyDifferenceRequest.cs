using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class deleteQtyDifferenceRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdQtyDifferenceRequest",
                schema: "dbo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdQtyDifferenceRequest",
                schema: "dbo",
                columns: table => new
                {
                    QtyDifferenceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeadPurchasingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadWarehouseManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QtyDifferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyDifferenceApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QtyDifferenceApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
        }
    }
}
