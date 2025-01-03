using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeProductReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WrhProductReturn",
                schema: "dbo",
                columns: table => new
                {
                    ProductReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductReturnNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Position1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserApprove1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveStatusUser1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Position2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserApprove2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveStatusUser2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Position3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserApprove3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveStatusUser3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageApprove1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageApprove2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageApprove3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_WrhProductReturn", x => x.ProductReturnId);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstDepartment_Department1Id",
                        column: x => x.Department1Id,
                        principalSchema: "dbo",
                        principalTable: "MstDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstDepartment_Department2Id",
                        column: x => x.Department2Id,
                        principalSchema: "dbo",
                        principalTable: "MstDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstDepartment_Department3Id",
                        column: x => x.Department3Id,
                        principalSchema: "dbo",
                        principalTable: "MstDepartment",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstPosition_Position1Id",
                        column: x => x.Position1Id,
                        principalSchema: "dbo",
                        principalTable: "MstPosition",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstPosition_Position2Id",
                        column: x => x.Position2Id,
                        principalSchema: "dbo",
                        principalTable: "MstPosition",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstPosition_Position3Id",
                        column: x => x.Position3Id,
                        principalSchema: "dbo",
                        principalTable: "MstPosition",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstUserActive_UserApprove1Id",
                        column: x => x.UserApprove1Id,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstUserActive_UserApprove2Id",
                        column: x => x.UserApprove2Id,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhProductReturn_MstUserActive_UserApprove3Id",
                        column: x => x.UserApprove3Id,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdProductReturnDetail",
                schema: "dbo",
                columns: table => new
                {
                    ProductReturnDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarehouseLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_OrdProductReturnDetail", x => x.ProductReturnDetailId);
                    table.ForeignKey(
                        name: "FK_OrdProductReturnDetail_WrhProductReturn_ProductReturnId",
                        column: x => x.ProductReturnId,
                        principalSchema: "dbo",
                        principalTable: "WrhProductReturn",
                        principalColumn: "ProductReturnId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdProductReturnDetail_ProductReturnId",
                schema: "dbo",
                table: "OrdProductReturnDetail",
                column: "ProductReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Department1Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Department1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Department2Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Department2Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Department3Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Department3Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Position1Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Position1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Position2Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Position2Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_Position3Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "Position3Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_UserAccessId",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_UserApprove1Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "UserApprove1Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_UserApprove2Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "UserApprove2Id");

            migrationBuilder.CreateIndex(
                name: "IX_WrhProductReturn_UserApprove3Id",
                schema: "dbo",
                table: "WrhProductReturn",
                column: "UserApprove3Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdProductReturnDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WrhProductReturn",
                schema: "dbo");
        }
    }
}
