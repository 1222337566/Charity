using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fdsfsdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetGroupName",
                table: "CharityProjectBeneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharityProjectActivityBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetGroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParticipationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Beneficiary"),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Unverified"),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectActivityBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectActivityBeneficiaries_CharityProjectBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityProjectBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityProjectActivityBeneficiaries_CharityProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectPhases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityProjectActivityBeneficiaries_CharityProjectSubGoalActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "CharityProjectSubGoalActivities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharityProjectActivityBeneficiaries_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectActivityBeneficiaryAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityBeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CharityProjectActivityBeneficiaryAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectActivityBeneficiaryAttachments_CharityProjectActivityBeneficiaries_ActivityBeneficiaryId",
                        column: x => x.ActivityBeneficiaryId,
                        principalTable: "CharityProjectActivityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaries_ActivityId_BeneficiaryId",
                table: "CharityProjectActivityBeneficiaries",
                columns: new[] { "ActivityId", "BeneficiaryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaries_BeneficiaryId",
                table: "CharityProjectActivityBeneficiaries",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaries_PhaseId",
                table: "CharityProjectActivityBeneficiaries",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaries_ProjectId",
                table: "CharityProjectActivityBeneficiaries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaries_VerificationStatus",
                table: "CharityProjectActivityBeneficiaries",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectActivityBeneficiaryAttachments_ActivityBeneficiaryId",
                table: "CharityProjectActivityBeneficiaryAttachments",
                column: "ActivityBeneficiaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityProjectActivityBeneficiaryAttachments");

            migrationBuilder.DropTable(
                name: "CharityProjectActivityBeneficiaries");

            migrationBuilder.DropColumn(
                name: "TargetGroupName",
                table: "CharityProjectBeneficiaries");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4537));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4554));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4558));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4561));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4563));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4571));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 16, 7, 48, 374, DateTimeKind.Utc).AddTicks(4573));
        }
    }
}
