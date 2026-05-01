using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fdsfsdf1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharityProjectTeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MemberType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Volunteer"),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParticipationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Assigned"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlannedDays = table.Column<int>(type: "int", nullable: true),
                    ActualDays = table.Column<int>(type: "int", nullable: true),
                    VerificationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Unverified"),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMembers_CharityHrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "CharityHrEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMembers_CharityProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMembers_CharityProjectSubGoalActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "CharityProjectSubGoalActivities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMembers_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMembers_CharityVolunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "CharityVolunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectTeamMemberAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectTeamMemberAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectTeamMemberAttachments_CharityProjectTeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "CharityProjectTeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8438));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8456));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8461));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8463));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8466));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8475));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 50, 28, 956, DateTimeKind.Utc).AddTicks(8477));

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMemberAttachments_TeamMemberId",
                table: "CharityProjectTeamMemberAttachments",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMembers_ActivityId",
                table: "CharityProjectTeamMembers",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMembers_EmployeeId",
                table: "CharityProjectTeamMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMembers_PhaseId",
                table: "CharityProjectTeamMembers",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMembers_ProjectId_MemberType",
                table: "CharityProjectTeamMembers",
                columns: new[] { "ProjectId", "MemberType" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectTeamMembers_VolunteerId",
                table: "CharityProjectTeamMembers",
                column: "VolunteerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityProjectTeamMemberAttachments");

            migrationBuilder.DropTable(
                name: "CharityProjectTeamMembers");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(6995));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7010));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7013));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7016));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7018));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7026));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 20, 39, 49, 951, DateTimeKind.Utc).AddTicks(7028));
        }
    }
}
