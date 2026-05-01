using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CharityProjectPhaseActivities_PhaseId_SortOrder",
                table: "CharityProjectPhaseActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ResponsibleRole",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NeededResources",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityCount",
                table: "CharityProjectProposalActivitys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedCount",
                table: "CharityProjectProposalActivitys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedDurationDays",
                table: "CharityProjectProposalActivitys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlannedHoursPerDay",
                table: "CharityProjectProposalActivitys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuantityUnit",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsible",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "CharityProjectProposalActivitys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetGroup",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ResponsiblePersonName",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProgressPercent",
                table: "CharityProjectPhaseActivities",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalPhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalPhases_CharityProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "CharityProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharityProjectProposalPhaseActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ContributionPercent = table.Column<int>(type: "int", nullable: false),
                    PlannedQuantity = table.Column<int>(type: "int", nullable: true),
                    DurationDays = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityProjectProposalPhaseActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityProjectProposalPhaseActivities_CharityProjectProposalPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "CharityProjectProposalPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1845));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1850));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1852));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1854));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1862));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 21, 30, 3, 717, DateTimeKind.Utc).AddTicks(1867));

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseActivities_PhaseId",
                table: "CharityProjectPhaseActivities",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalPhaseActivities_PhaseId",
                table: "CharityProjectProposalPhaseActivities",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectProposalPhases_ProjectProposalId",
                table: "CharityProjectProposalPhases",
                column: "ProjectProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityProjectProposalPhaseActivities");

            migrationBuilder.DropTable(
                name: "CharityProjectProposalPhases");

            migrationBuilder.DropIndex(
                name: "IX_CharityProjectPhaseActivities_PhaseId",
                table: "CharityProjectPhaseActivities");

            migrationBuilder.DropColumn(
                name: "ActivityCount",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "PlannedCount",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "PlannedDurationDays",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "PlannedHoursPerDay",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "QuantityUnit",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "Responsible",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.DropColumn(
                name: "TargetGroup",
                table: "CharityProjectProposalActivitys");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "ResponsibleRole",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NeededResources",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharityProjectProposalActivitys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ResponsiblePersonName",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProgressPercent",
                table: "CharityProjectPhaseActivities",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CharityProjectPhaseActivities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4492));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4507));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4514));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4517));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4520));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4532));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 27, 10, 55, 22, 502, DateTimeKind.Utc).AddTicks(4534));

            migrationBuilder.CreateIndex(
                name: "IX_CharityProjectPhaseActivities_PhaseId_SortOrder",
                table: "CharityProjectPhaseActivities",
                columns: new[] { "PhaseId", "SortOrder" });
        }
    }
}
