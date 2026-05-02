using InfrastrfuctureManagmentCore.Domains.HR.Advanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced
{
    public interface IHrPerformanceEvaluationRepository
    {
        Task<List<HrPerformanceEvaluation>> GetAllAsync(Guid? employeeId = null);
        Task<HrPerformanceEvaluation?> GetByIdAsync(Guid id);
        Task AddAsync(HrPerformanceEvaluation entity);
        Task UpdateAsync(HrPerformanceEvaluation entity);
    }
}
