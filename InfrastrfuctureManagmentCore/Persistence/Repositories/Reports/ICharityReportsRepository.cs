using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Reports
{
    public interface ICharityReportsRepository
    {
        Task<List<BeneficiaryStatusReportRowDto>> GetBeneficiaryStatusReportAsync();
        Task<List<DonationReportRowDto>> GetDonationReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<ProjectFinancialReportRowDto>> GetProjectFinancialReportAsync();
        Task<List<PayrollMonthReportRowDto>> GetPayrollMonthReportAsync();
        Task<List<StoreMovementReportRowDto>> GetStoreMovementReportAsync(DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<BeneficiaryAidDetailReportRowDto>> GetBeneficiaryAidDetailReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<DonorStatementReportRowDto>> GetDonorStatementReportAsync(Guid? donorId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<ProjectDetailsReportRowDto>> GetProjectDetailsReportAsync(Guid? projectId = null);
        Task<List<PayrollEmployeeDetailReportRowDto>> GetPayrollEmployeeDetailReportAsync(Guid? payrollMonthId = null);
        Task<List<StoreItemMovementDetailReportRowDto>> GetStoreItemMovementDetailReportAsync(Guid? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
