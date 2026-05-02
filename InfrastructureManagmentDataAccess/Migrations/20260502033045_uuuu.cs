using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class uuuu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "CharityGrantInstallments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CharityDynamicWorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDynamicWorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityUserWorkspaceLayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LayoutJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityUserWorkspaceLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityDynamicWorkflowStepTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssignedRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityDynamicWorkflowStepTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityDynamicWorkflowStepTemplates_CharityDynamicWorkflowDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "CharityDynamicWorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8353));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8377));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8381));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8383));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8385));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8399));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 3, 30, 43, 140, DateTimeKind.Utc).AddTicks(8401));

            migrationBuilder.CreateIndex(
                name: "IX_CharityDynamicWorkflowDefinitions_EntityType_IsActive",
                table: "CharityDynamicWorkflowDefinitions",
                columns: new[] { "EntityType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityDynamicWorkflowStepTemplates_DefinitionId_StepOrder",
                table: "CharityDynamicWorkflowStepTemplates",
                columns: new[] { "DefinitionId", "StepOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityUserWorkspaceLayouts_UserId",
                table: "CharityUserWorkspaceLayouts",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityDynamicWorkflowStepTemplates");

            migrationBuilder.DropTable(
                name: "CharityUserWorkspaceLayouts");

            migrationBuilder.DropTable(
                name: "CharityDynamicWorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "CharityGrantInstallments");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7776));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7795));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7800));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7802));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7804));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7818));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 21, 31, 31, 310, DateTimeKind.Utc).AddTicks(7821));
        }
    }
}
