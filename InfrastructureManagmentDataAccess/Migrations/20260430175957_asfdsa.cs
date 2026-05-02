using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureManagmentDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class asfdsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AidRequestLineDescriptionSnapshot",
                table: "CharityStockNeedRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedTotalValue",
                table: "CharityStockNeedRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "CharityStockNeedRequestLines",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedTotalValue",
                table: "CharityStockNeedRequestLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedUnitValue",
                table: "CharityStockNeedRequestLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemNameSnapshot",
                table: "CharityStockNeedRequestLines",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CharityStockNeedRequests_BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests",
                column: "BeneficiaryAidRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityStockNeedRequests_BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests",
                column: "BeneficiaryAidRequestLineId",
                unique: true,
                filter: "[BeneficiaryAidRequestLineId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityStockNeedRequests_BeneficiaryAidRequestLines_BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests",
                column: "BeneficiaryAidRequestLineId",
                principalTable: "BeneficiaryAidRequestLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityStockNeedRequests_CharityAidRequests_BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests",
                column: "BeneficiaryAidRequestId",
                principalTable: "CharityAidRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityStockNeedRequests_BeneficiaryAidRequestLines_BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CharityStockNeedRequests_CharityAidRequests_BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropIndex(
                name: "IX_CharityStockNeedRequests_BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropIndex(
                name: "IX_CharityStockNeedRequests_BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "AidRequestLineDescriptionSnapshot",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAidRequestId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAidRequestLineId",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedTotalValue",
                table: "CharityStockNeedRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedTotalValue",
                table: "CharityStockNeedRequestLines");

            migrationBuilder.DropColumn(
                name: "EstimatedUnitValue",
                table: "CharityStockNeedRequestLines");

            migrationBuilder.DropColumn(
                name: "ItemNameSnapshot",
                table: "CharityStockNeedRequestLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "CharityStockNeedRequestLines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7247));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000002-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7273));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000003-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7277));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000004-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7279));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000005-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7282));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000006-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7296));

            migrationBuilder.UpdateData(
                table: "HrLeaveTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1000007-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 4, 30, 9, 51, 57, 574, DateTimeKind.Utc).AddTicks(7298));
        }
    }
}
