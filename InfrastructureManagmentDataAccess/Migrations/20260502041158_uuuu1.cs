using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class uuuu1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grantors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grantors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DocumentDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedEntityName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFundingAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgreementNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    GrantorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FundingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFundingAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFundingAgreements_CharityProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CharityProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectFundingAgreements_Grantors_GrantorId",
                        column: x => x.GrantorId,
                        principalTable: "Grantors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFundingInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectFundingAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstallmentNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiptDocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFundingInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFundingInstallments_ProjectFundingAgreements_ProjectFundingAgreementId",
                        column: x => x.ProjectFundingAgreementId,
                        principalTable: "ProjectFundingAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8403));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8417));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8421));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8423));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8426));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8439));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 2, 4, 11, 56, 598, DateTimeKind.Utc).AddTicks(8441));

            migrationBuilder.CreateIndex(
                name: "IX_Grantors_GrantorCode",
                table: "Grantors",
                column: "GrantorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grantors_IsActive",
                table: "Grantors",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Grantors_NameAr",
                table: "Grantors",
                column: "NameAr");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDocuments_DocumentDateUtc",
                table: "OrganizationDocuments",
                column: "DocumentDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDocuments_DocumentNumber",
                table: "OrganizationDocuments",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDocuments_DocumentType",
                table: "OrganizationDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDocuments_IsArchived",
                table: "OrganizationDocuments",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDocuments_RelatedEntityType_RelatedEntityId",
                table: "OrganizationDocuments",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingAgreements_AgreementNumber",
                table: "ProjectFundingAgreements",
                column: "AgreementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingAgreements_GrantorId",
                table: "ProjectFundingAgreements",
                column: "GrantorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingAgreements_ProjectId",
                table: "ProjectFundingAgreements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingAgreements_Status",
                table: "ProjectFundingAgreements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingInstallments_DueDateUtc",
                table: "ProjectFundingInstallments",
                column: "DueDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingInstallments_ProjectFundingAgreementId",
                table: "ProjectFundingInstallments",
                column: "ProjectFundingAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundingInstallments_Status",
                table: "ProjectFundingInstallments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationDocuments");

            migrationBuilder.DropTable(
                name: "ProjectFundingInstallments");

            migrationBuilder.DropTable(
                name: "ProjectFundingAgreements");

            migrationBuilder.DropTable(
                name: "Grantors");

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
        }
    }
}
