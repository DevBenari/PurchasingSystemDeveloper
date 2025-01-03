using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class initializeApprovalProductReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WrhApprovalProductReturn",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalProductReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductReturnNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_WrhApprovalProductReturn", x => x.ApprovalProductReturnId);
                    table.ForeignKey(
                        name: "FK_WrhApprovalProductReturn_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalProductReturn_MstUserActive_UserApproveId",
                        column: x => x.UserApproveId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrhApprovalProductReturn_WrhProductReturn_ProductReturnId",
                        column: x => x.ProductReturnId,
                        principalSchema: "dbo",
                        principalTable: "WrhProductReturn",
                        principalColumn: "ProductReturnId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalProductReturn_ProductReturnId",
                schema: "dbo",
                table: "WrhApprovalProductReturn",
                column: "ProductReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalProductReturn_UserAccessId",
                schema: "dbo",
                table: "WrhApprovalProductReturn",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_WrhApprovalProductReturn_UserApproveId",
                schema: "dbo",
                table: "WrhApprovalProductReturn",
                column: "UserApproveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrhApprovalProductReturn",
                schema: "dbo");
        }
    }
}
