using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectAccountingReportRepository
    {
        Task<ProjectCostSummaryDto> GetProjectCostSummaryAsync(Guid? projectId, Guid? costCenterId, DateTime? fromDate, DateTime? toDate);
        Task<ProjectLedgerDto?> GetProjectLedgerAsync(Guid projectId, DateTime? fromDate, DateTime? toDate);
    }
}
