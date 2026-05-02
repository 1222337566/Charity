using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingCashBankVouchers");

            migrationBuilder.DropTable(
                name: "AccountingTreasuryBankTransfers");

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

            migrationBuilder.CreateTable(
                name: "BeneficiaryAidRequestLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AidRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    EstimatedUnitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedTotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FulfillmentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "CashEquivalent"),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryAidRequestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeneficiaryAidRequestLines_CharityAidRequests_AidRequestId",
                        column: x => x.AidRequestId,
                        principalTable: "CharityAidRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficiaryAidRequestLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeneficiaryAidRequestLines_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryAidRequestLines_AidRequestId",
                table: "BeneficiaryAidRequestLines",
                column: "AidRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryAidRequestLines_FulfillmentMethod",
                table: "BeneficiaryAidRequestLines",
                column: "FulfillmentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryAidRequestLines_ItemId",
                table: "BeneficiaryAidRequestLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryAidRequestLines_WarehouseId",
                table: "BeneficiaryAidRequestLines",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeneficiaryAidRequestLines");

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

            migrationBuilder.CreateTable(
                name: "AccountingCashBankVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OppositeAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingCashBankVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingCashBankVouchers_FinacialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingCashBankVouchers_FinacialAccounts_OppositeAccountId",
                        column: x => x.OppositeAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingTreasuryBankTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromFinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToFinancialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTreasuryBankTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTreasuryBankTransfers_FinacialAccounts_FromFinancialAccountId",
                        column: x => x.FromFinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingTreasuryBankTransfers_FinacialAccounts_ToFinancialAccountId",
                        column: x => x.ToFinancialAccountId,
                        principalTable: "FinacialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4445));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4461));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4464));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4467));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4469));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4479));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 29, 20, 50, 46, 458, DateTimeKind.Utc).AddTicks(4482));

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCashBankVouchers_FinancialAccountId",
                table: "AccountingCashBankVouchers",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCashBankVouchers_OppositeAccountId",
                table: "AccountingCashBankVouchers",
                column: "OppositeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCashBankVouchers_VoucherNumber",
                table: "AccountingCashBankVouchers",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTreasuryBankTransfers_FromFinancialAccountId",
                table: "AccountingTreasuryBankTransfers",
                column: "FromFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTreasuryBankTransfers_ToFinancialAccountId",
                table: "AccountingTreasuryBankTransfers",
                column: "ToFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTreasuryBankTransfers_TransferNumber",
                table: "AccountingTreasuryBankTransfers",
                column: "TransferNumber",
                unique: true);
        }
    }
}
