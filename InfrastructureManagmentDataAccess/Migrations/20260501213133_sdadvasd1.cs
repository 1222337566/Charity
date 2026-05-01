using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdadvasd1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                table: "AccountingFiscalPeriods",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAtUtc",
                table: "AccountingFiscalPeriods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedByUserId",
                table: "AccountingFiscalPeriods",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosingNotes",
                table: "AccountingFiscalPeriods",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountingFiscalPeriods_IsClosed",
                table: "AccountingFiscalPeriods",
                column: "IsClosed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountingFiscalPeriods_IsClosed",
                table: "AccountingFiscalPeriods");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "AccountingFiscalPeriods");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "AccountingFiscalPeriods");

            migrationBuilder.DropColumn(
                name: "ClosingNotes",
                table: "AccountingFiscalPeriods");

            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                table: "AccountingFiscalPeriods",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4224));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4242));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4245));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4248));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4250));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4272));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 5, 1, 20, 12, 35, 368, DateTimeKind.Utc).AddTicks(4275));
        }
    }
}
