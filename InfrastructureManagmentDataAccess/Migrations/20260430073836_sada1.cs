using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sada1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AidRequestLineId",
                table: "CharityDonationAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityDonationAllocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityDonationAllocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AidRequestLineId",
                table: "CharityAidDisbursements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8490));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8505));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8517));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8519));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8522));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8531));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 38, 34, 488, DateTimeKind.Utc).AddTicks(8533));

            migrationBuilder.CreateIndex(
                name: "IX_CharityDonationAllocations_AidRequestLineId",
                table: "CharityDonationAllocations",
                column: "AidRequestLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityAidDisbursements_AidRequestLineId",
                table: "CharityAidDisbursements",
                column: "AidRequestLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityAidDisbursements_BeneficiaryAidRequestLines_AidRequestLineId",
                table: "CharityAidDisbursements",
                column: "AidRequestLineId",
                principalTable: "BeneficiaryAidRequestLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityDonationAllocations_BeneficiaryAidRequestLines_AidRequestLineId",
                table: "CharityDonationAllocations",
                column: "AidRequestLineId",
                principalTable: "BeneficiaryAidRequestLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityAidDisbursements_BeneficiaryAidRequestLines_AidRequestLineId",
                table: "CharityAidDisbursements");

            migrationBuilder.DropForeignKey(
                name: "FK_CharityDonationAllocations_BeneficiaryAidRequestLines_AidRequestLineId",
                table: "CharityDonationAllocations");

            migrationBuilder.DropIndex(
                name: "IX_CharityDonationAllocations_AidRequestLineId",
                table: "CharityDonationAllocations");

            migrationBuilder.DropIndex(
                name: "IX_CharityAidDisbursements_AidRequestLineId",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "AidRequestLineId",
                table: "CharityDonationAllocations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityDonationAllocations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityDonationAllocations");

            migrationBuilder.DropColumn(
                name: "AidRequestLineId",
                table: "CharityAidDisbursements");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3549));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3564));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3567));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3569));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3572));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3581));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 7, 22, 34, 53, DateTimeKind.Utc).AddTicks(3583));
        }
    }
}
