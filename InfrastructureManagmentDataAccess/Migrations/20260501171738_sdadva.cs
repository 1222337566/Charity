using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdadva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "StockTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "CharityStoreReceipts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "CharityStoreIssues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "CharityStoreIssues",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9896));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9913));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9916));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9919));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9922));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9933));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 17, 17, 36, 538, DateTimeKind.Utc).AddTicks(9935));

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ReferenceId",
                table: "StockTransactions",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ReferenceType_ReferenceId",
                table: "StockTransactions",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreReceipts_SourceType_SourceId",
                table: "CharityStoreReceipts",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_CharityStoreIssues_SourceType_SourceId",
                table: "CharityStoreIssues",
                columns: new[] { "SourceType", "SourceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_ReferenceId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_ReferenceType_ReferenceId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CharityStoreReceipts_SourceType_SourceId",
                table: "CharityStoreReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CharityStoreIssues_SourceType_SourceId",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "CharityStoreReceipts");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "CharityStoreIssues");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "CharityStoreIssues");

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
        }
    }
}
