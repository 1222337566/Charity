using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastructureManagmentServices.CharityReports;

public interface ICharityReportExportService
{
    byte[] ExportBeneficiaryStatusesExcel(IEnumerable<BeneficiaryStatusReportRowDto> rows);
    byte[] ExportBeneficiaryStatusesPdf(IEnumerable<BeneficiaryStatusReportRowDto> rows);

    byte[] ExportDonationsExcel(IEnumerable<DonationReportRowDto> rows, DateTime? fromDate, DateTime? toDate);
    byte[] ExportDonationsPdf(IEnumerable<DonationReportRowDto> rows, DateTime? fromDate, DateTime? toDate);

    byte[] ExportProjectsExcel(IEnumerable<ProjectFinancialReportRowDto> rows);
    byte[] ExportProjectsPdf(IEnumerable<ProjectFinancialReportRowDto> rows);

    byte[] ExportPayrollExcel(IEnumerable<PayrollMonthReportRowDto> rows);
    byte[] ExportPayrollPdf(IEnumerable<PayrollMonthReportRowDto> rows);

    byte[] ExportStoreMovementsExcel(IEnumerable<StoreMovementReportRowDto> rows, DateTime? fromDate, DateTime? toDate);
    byte[] ExportStoreMovementsPdf(IEnumerable<StoreMovementReportRowDto> rows, DateTime? fromDate, DateTime? toDate);
}
