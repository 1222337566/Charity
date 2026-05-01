using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sdadvasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalesInvoiceId",
                table: "CustomerReceipts",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_SalesInvoiceId",
                table: "CustomerReceipts",
                column: "SalesInvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReceipts_SalesInvoices_SalesInvoiceId",
                table: "CustomerReceipts",
                column: "SalesInvoiceId",
                principalTable: "SalesInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReceipts_SalesInvoices_SalesInvoiceId",
                table: "CustomerReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReceipts_SalesInvoiceId",
                table: "CustomerReceipts");

            migrationBuilder.DropColumn(
                name: "SalesInvoiceId",
                table: "CustomerReceipts");

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
        }
    }
}
