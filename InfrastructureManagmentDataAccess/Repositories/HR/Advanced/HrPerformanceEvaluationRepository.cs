using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Advanced
{
    public class HrPerformanceEvaluationRepository : IHrPerformanceEvaluationRepository
    {
        private readonly AppDbContext _db;
        public HrPerformanceEvaluationRepository(AppDbContext db) => _db = db;

        public async Task<List<HrPerformanceEvaluation>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrPerformanceEvaluation>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.EvaluatorEmployee)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);

            return await query.OrderByDescending(x => x.EvaluationDate).ToListAsync();
        }

        public Task<HrPerformanceEvaluation?> GetByIdAsync(Guid id)
            => _db.Set<HrPerformanceEvaluation>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrPerformanceEvaluation entity)
        {
            _db.Set<HrPerformanceEvaluation>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrPerformanceEvaluation entity)
        {
            _db.Set<HrPerformanceEvaluation>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
