using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingSystemDeveloper.Migrations
{
    public partial class renameApprovalToApprovalPurchaseRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdApproval",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "OrdApprovalPurchaseRequest",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiredDay = table.Column<int>(type: "int", nullable: false),
                    RemainingDay = table.Column<int>(type: "int", nullable: false),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
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
                    table.PrimaryKey("PK_OrdApprovalPurchaseRequest", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_OrdApprovalPurchaseRequest_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApprovalPurchaseRequest_MstUserActive_UserApproveId",
                        column: x => x.UserApproveId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApprovalPurchaseRequest_OrdPurchaseRequest_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseRequest",
                        principalColumn: "PurchaseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalPurchaseRequest_PurchaseRequestId",
                schema: "dbo",
                table: "OrdApprovalPurchaseRequest",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalPurchaseRequest_UserAccessId",
                schema: "dbo",
                table: "OrdApprovalPurchaseRequest",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApprovalPurchaseRequest_UserApproveId",
                schema: "dbo",
                table: "OrdApprovalPurchaseRequest",
                column: "UserApproveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdApprovalPurchaseRequest",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "OrdApproval",
                schema: "dbo",
                columns: table => new
                {
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAccessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserApproveId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ApprovalStatusUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiredDay = table.Column<int>(type: "int", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemainingDay = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdApproval", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_OrdApproval_AspNetUsers_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApproval_MstUserActive_UserApproveId",
                        column: x => x.UserApproveId,
                        principalSchema: "dbo",
                        principalTable: "MstUserActive",
                        principalColumn: "UserActiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdApproval_OrdPurchaseRequest_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "dbo",
                        principalTable: "OrdPurchaseRequest",
                        principalColumn: "PurchaseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_PurchaseRequestId",
                schema: "dbo",
                table: "OrdApproval",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserAccessId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdApproval_UserApproveId",
                schema: "dbo",
                table: "OrdApproval",
                column: "UserApproveId");
        }
    }
}
