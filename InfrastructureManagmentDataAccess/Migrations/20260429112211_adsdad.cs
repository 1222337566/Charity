using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adsdad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "PurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "PurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FinacialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "FinacialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "FinacialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "FinacialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CustomerReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CustomerReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CustomerReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CustomerReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserName",
                table: "CharityStoreReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityStoreReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByUserName",
                table: "CharityStoreReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityStoreReceipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityStoreReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityStoreReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserName",
                table: "CharityStoreIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityStoreIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByUserName",
                table: "CharityStoreIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityStoreIssues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityStoreIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityStoreIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CharityStockReturnVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityStockReturnVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityStockReturnVouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityStockReturnVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityStockReturnVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserName",
                table: "CharityStockNeedRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityStockNeedRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityStockNeedRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityStockNeedRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CharityStockDisposalVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityStockDisposalVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityStockDisposalVouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityStockDisposalVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityStockDisposalVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityDonations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityDonations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityDonations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CharityBoardDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityBoardDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityBoardDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityBoardDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityAidRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityAidRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityAidRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityAidRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserName",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutedByUserName",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByUserName",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CharityAidDisbursements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CharityAidDisbursements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "AccountingJournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "AccountingJournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostedByUserName",
                table: "AccountingJournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "AccountingJournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "AccountingJournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2582));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2598));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2602));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2605));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2608));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2616));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 11, 22, 9, 340, DateTimeKind.Utc).AddTicks(2619));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FinacialAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "FinacialAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "FinacialAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "FinacialAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CustomerReceipts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CustomerReceipts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CustomerReceipts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CustomerReceipts");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserName",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "RejectedByUserName",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserName",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "RejectedByUserName",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CharityStockReturnVouchers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityStockReturnVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityStockReturnVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityStockReturnVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityStockReturnVouchers");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserName",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CharityStockDisposalVouchers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityStockDisposalVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityStockDisposalVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityStockDisposalVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityStockDisposalVouchers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityDonations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityDonations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityDonations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CharityBoardDecisions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityBoardDecisions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityBoardDecisions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityBoardDecisions");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityBeneficiaryCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityAidRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityAidRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityAidRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityAidRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserName",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "ExecutedByUserName",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "RejectedByUserName",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CharityAidDisbursements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AccountingJournalEntries");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "AccountingJournalEntries");

            migrationBuilder.DropColumn(
                name: "PostedByUserName",
                table: "AccountingJournalEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AccountingJournalEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "AccountingJournalEntries");

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6223));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6238));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6242));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6244));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6246));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6253));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 28, 22, 48, 45, 411, DateTimeKind.Utc).AddTicks(6256));
        }
    }
}
