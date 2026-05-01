using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Reports
{
    public interface ICharityOperationsRepository
    {
        Task<CharityWorkspaceSnapshotDto> GetWorkspaceSnapshotAsync(DateTime? today = null);
    }
}
