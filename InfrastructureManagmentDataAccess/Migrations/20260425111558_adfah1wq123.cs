using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adfah1wq123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrLeaveTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MaxDaysPerYear = table.Column<int>(type: "int", nullable: false),
                    MaxConsecutiveDays = table.Column<int>(type: "int", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    PaidLeave = table.Column<bool>(type: "bit", nullable: false),
                    CarryOverAllowed = table.Column<bool>(type: "bit", nullable: false),
                    MaxCarryOverDays = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HrLeaveBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalEntitled = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TotalUsed = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TotalPending = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    CarriedOver = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLeaveBalances_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrLeaveBalances_HrLeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "HrLeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrLeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    HalfDayPeriod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnedEarly = table.Column<bool>(type: "bit", nullable: false),
                    ReturnNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLeaveRequests_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrLeaveRequests_HrLeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "HrLeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "HrLeaveTypes",
                columns: new[] { "Id", "CarryOverAllowed", "Category", "Code", "Color", "CreatedAtUtc", "IsActive", "MaxCarryOverDays", "MaxConsecutiveDays", "MaxDaysPerYear", "NameAr", "NameEn", "PaidLeave", "RequiresAttachment" },
                values: new object[,]
                {
                    { new Guid("a1000001-0000-0000-0000-000000000001"), true, "Annual", "ANNUAL", "#0e6f73", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9898), true, 7, 21, 21, "إجازة سنوية", "Annual Leave", true, false },
                    { new Guid("a1000002-0000-0000-0000-000000000002"), false, "Sick", "SICK", "#f46a6a", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9918), true, 0, 30, 90, "إجازة مرضية", "Sick Leave", true, true },
                    { new Guid("a1000003-0000-0000-0000-000000000003"), false, "Emergency", "EMRG", "#f1b44c", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9921), true, 0, 3, 6, "إجازة طارئة", "Emergency Leave", true, false },
                    { new Guid("a1000004-0000-0000-0000-000000000004"), false, "Unpaid", "UNPAID", "#adb5bd", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9924), true, 0, 30, 60, "إجازة بدون راتب", "Unpaid Leave", false, false },
                    { new Guid("a1000005-0000-0000-0000-000000000005"), false, "Maternity", "MAT", "#f472b6", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9936), true, 0, 90, 90, "إجازة أمومة", "Maternity Leave", true, false },
                    { new Guid("a1000006-0000-0000-0000-000000000006"), false, "Paternity", "PAT", "#818cf8", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9944), true, 0, 3, 3, "إجازة أبوة", "Paternity Leave", true, false },
                    { new Guid("a1000007-0000-0000-0000-000000000007"), false, "Hajj", "HAJJ", "#34c38f", new DateTime(2026, 4, 25, 11, 15, 56, 63, DateTimeKind.Utc).AddTicks(9947), true, 0, 30, 30, "إجازة الحج", "Hajj Leave", true, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveBalances_EmployeeId_LeaveTypeId_Year",
                table: "HrLeaveBalances",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveBalances_LeaveTypeId",
                table: "HrLeaveBalances",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_EmployeeId",
                table: "HrLeaveRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_EmployeeId_StartDate",
                table: "HrLeaveRequests",
                columns: new[] { "EmployeeId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_LeaveTypeId",
                table: "HrLeaveRequests",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_Status",
                table: "HrLeaveRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveTypes_Code",
                table: "HrLeaveTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrLeaveBalances");

            migrationBuilder.DropTable(
                name: "HrLeaveRequests");

            migrationBuilder.DropTable(
                name: "HrLeaveTypes");
        }
    }
}
