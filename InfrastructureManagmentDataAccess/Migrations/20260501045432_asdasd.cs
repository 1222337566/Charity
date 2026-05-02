using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsWaitingListCategory = table.Column<bool>(type: "bit", nullable: false),
                    RequiresDisabilityType = table.Column<bool>(type: "bit", nullable: false),
                    IsProjectRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsActivityRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharityBeneficiaryCategoryAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityBeneficiaryCategoryAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryCategoryAssignments_CharityBeneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "CharityBeneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharityBeneficiaryCategoryAssignments_CharityBeneficiaryCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CharityBeneficiaryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8748));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8751));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8757));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8759));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8768));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 4, 54, 29, 994, DateTimeKind.Utc).AddTicks(8771));

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategories_Code",
                table: "CharityBeneficiaryCategories",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategories_IsActive",
                table: "CharityBeneficiaryCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategories_IsWaitingListCategory",
                table: "CharityBeneficiaryCategories",
                column: "IsWaitingListCategory");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategories_NameAr",
                table: "CharityBeneficiaryCategories",
                column: "NameAr");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_AssignedAtUtc",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "AssignedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_BeneficiaryId",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_BeneficiaryId_CategoryId_ProjectId_ProjectActivityId_Status",
                table: "CharityBeneficiaryCategoryAssignments",
                columns: new[] { "BeneficiaryId", "CategoryId", "ProjectId", "ProjectActivityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_CategoryId",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_ProjectActivityId",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "ProjectActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_ProjectId",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityBeneficiaryCategoryAssignments_Status",
                table: "CharityBeneficiaryCategoryAssignments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityBeneficiaryCategoryAssignments");

            migrationBuilder.DropTable(
                name: "CharityBeneficiaryCategories");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1901));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1921));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1925));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1927));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1929));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1937));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 17, 59, 54, 656, DateTimeKind.Utc).AddTicks(1940));
        }
    }
}
