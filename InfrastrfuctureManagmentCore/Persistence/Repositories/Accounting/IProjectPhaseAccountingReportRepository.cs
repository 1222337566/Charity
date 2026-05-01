using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectPhaseAccountingReportRepository
    {
        Task<ProjectPhaseCostSummaryDto> GetPhaseCostSummaryAsync(Guid? projectId, Guid? phaseId, DateTime? fromDate, DateTime? toDate);
        Task<ProjectPhaseLedgerDto?> GetPhaseLedgerAsync(Guid phaseId, DateTime? fromDate, DateTime? toDate);
        Task<ProjectPhaseAlertDto> GetAlertsAsync(Guid? projectId, DateTime? asOfDate);
    }
}
