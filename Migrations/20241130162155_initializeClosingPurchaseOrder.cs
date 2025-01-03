using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeClosingPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c1fb17a-b16f-4877-b63e-0c981fce4e55");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29667799-9a77-476f-a4a2-f98f16e3413e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3155ad0b-3ff9-4336-80a9-a83f4d619392");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49b394a6-6d27-4872-932e-e3a50a075808");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ba73d55-d106-4bf6-8097-8e2b9ee5fd47");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6815db3b-87e3-482b-a75a-e0555364d54c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73f28f66-00fe-479d-835f-83a46a2a1274");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c65bb85-5e35-4306-a0d6-594c94ac0f94");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90e4a277-18cb-4c89-9800-e94a0a87e64e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91331c73-4934-4010-b7a6-8c78ec7e0799");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9225dc73-b091-4658-ab5c-67e10ca8f194");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "939a0bfa-0dd5-4d4c-81d9-c177ed1fb622");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "944116d7-c153-449e-97a7-51fa0f835d45");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95d13c25-2b6d-4667-bbe2-8d149c20ef93");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "962a8321-789d-488c-97ba-e4bb60feb9ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "973250b1-e8f7-4a25-aece-350724c7281a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98efd437-dd11-4ad9-966a-dea874e5eeed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99471211-2e67-4b1d-a3d4-dfbaacf36586");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99516357-af9d-42f4-992f-f038e6128b4a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "996aa5d5-ca72-4b4b-abc1-4135cdb8cf76");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "997984b2-3085-4ce5-b91a-88f6537e13ea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99808c34-0ba4-49c4-afd7-96d1ae9dd55e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9999707a-96a7-4fba-87ce-09ceac2bb27b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "999d95b7-81ec-4ca1-b565-021c2f42befd");

            migrationBuilder.CreateTable(
                name: "RptClosingPurchaseOrder",
                schema: "dbo",
                columns: table => new
                {
                    ClosingPurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosingPurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalPo = table.Column<int>(type: "int", nullable: false),
                    TotalQty = table.Column<int>(type: "int", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_RptClosingPurchaseOrder", x => x.ClosingPurchaseOrderId);
                    table.ForeignKey(
                        name: "FK_RptClosingPurchaseOrder_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RptClosingPurchaseOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    ClosingPurchaseOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosingPurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermOfPaymentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_RptClosingPurchaseOrderDetail", x => x.ClosingPurchaseOrderDetailId);
                    table.ForeignKey(
                        name: "FK_RptClosingPurchaseOrderDetail_RptClosingPurchaseOrder_ClosingPurchaseOrderId",
                        column: x => x.ClosingPurchaseOrderId,
                        principalSchema: "dbo",
                        principalTable: "RptClosingPurchaseOrder",
                        principalColumn: "ClosingPurchaseOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RptClosingPurchaseOrder_UserAccessId",
                schema: "dbo",
                table: "RptClosingPurchaseOrder",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_RptClosingPurchaseOrderDetail_ClosingPurchaseOrderId",
                schema: "dbo",
                table: "RptClosingPurchaseOrderDetail",
                column: "ClosingPurchaseOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RptClosingPurchaseOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RptClosingPurchaseOrder",
                schema: "dbo");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1c1fb17a-b16f-4877-b63e-0c981fce4e55", "DASHBOARD", "Dashboard", "DASHBOARD" },
                    { "29667799-9a77-476f-a4a2-f98f16e3413e", "MASTER DATA", "Dashboard Master Data", "DASHBOARD MASTER DATA" },
                    { "3155ad0b-3ff9-4336-80a9-a83f4d619392", "MASTER DATA", "Master Data", "MASTER DATA" },
                    { "49b394a6-6d27-4872-932e-e3a50a075808", "MASTER DATA", "Role", "ROLE" },
                    { "5ba73d55-d106-4bf6-8097-8e2b9ee5fd47", "MASTER DATA", "Users", "USERS" },
                    { "6815db3b-87e3-482b-a75a-e0555364d54c", "MASTER DATA", "Bank", "BANK" },
                    { "73f28f66-00fe-479d-835f-83a46a2a1274", "MASTER DATA", "Lead Time", "LEAD TIME" },
                    { "8c65bb85-5e35-4306-a0d6-594c94ac0f94", "MASTER DATA", "Product", "PRODUCT" },
                    { "90e4a277-18cb-4c89-9800-e94a0a87e64e", "MASTER DATA", "Warehouse", "WAREHOUSE" },
                    { "91331c73-4934-4010-b7a6-8c78ec7e0799", "MASTER DATA", "Supplier", "SUPPLIER" },
                    { "9225dc73-b091-4658-ab5c-67e10ca8f194", "MASTER DATA", "Get Data API", "GET DATA API" },
                    { "939a0bfa-0dd5-4d4c-81d9-c177ed1fb622", "MASTER DATA", "Term Of Payment", "TERM OF PAYMENT" },
                    { "944116d7-c153-449e-97a7-51fa0f835d45", "MASTER DATA", "Calculate Min-Max Stock", "CALCULATE MIN-MAX STOCK" },
                    { "95d13c25-2b6d-4667-bbe2-8d149c20ef93", "UNIT REQUEST", "Unit Request", "UNIT REQUEST" },
                    { "962a8321-789d-488c-97ba-e4bb60feb9ef", "APPROVAL PURCHASE REQ", "Approval Purchase Req", "APPROVAL PURCHASE REQ" },
                    { "973250b1-e8f7-4a25-aece-350724c7281a", "7cf929a2-f126-4179-84ff-5366f63f6ff7", "IndexRole", "INDEXROLE" },
                    { "98efd437-dd11-4ad9-966a-dea874e5eeed", "STOCK MONITORING", "Stock Monitoring", "STOCK MONITORING" },
                    { "99471211-2e67-4b1d-a3d4-dfbaacf36586", "QTY DIFFERENCE", "Qty Difference", "QTY DIFFERENCE" },
                    { "99516357-af9d-42f4-992f-f038e6128b4a", "PURCHASE ORDER", "Purchase Order", "PURCHASE ORDER" },
                    { "996aa5d5-ca72-4b4b-abc1-4135cdb8cf76", "APPROVAL UNIT REQUEST", "Approval Unit Request", "APPROVAL UNIT REQUEST" },
                    { "997984b2-3085-4ce5-b91a-88f6537e13ea", "KPI", "KPI", "KPI" },
                    { "99808c34-0ba4-49c4-afd7-96d1ae9dd55e", "PURCHASE REQUEST", "Purchase Request", "PURCHASE REQUEST" },
                    { "9999707a-96a7-4fba-87ce-09ceac2bb27b", "REPORT", "Report", "REPORT" },
                    { "999d95b7-81ec-4ca1-b565-021c2f42befd", "RECEIVE ORDER", "Receive Order", "RECEIVE ORDER" }
                });
        }
    }
}
