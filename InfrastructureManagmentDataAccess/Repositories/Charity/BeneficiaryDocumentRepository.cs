using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity;

public class BeneficiaryDocumentRepository : IBeneficiaryDocumentRepository
{
    private readonly AppDbContext _db;

    public BeneficiaryDocumentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BeneficiaryDocument>> GetListAsync(Guid? beneficiaryId = null)
    {
        var q = _db.Set<BeneficiaryDocument>()
            .Include(x => x.Beneficiary)
            .AsNoTracking()
            .AsQueryable();

        if (beneficiaryId.HasValue)
            q = q.Where(x => x.BeneficiaryId == beneficiaryId.Value);

        return await q.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }

    public Task<List<BeneficiaryDocument>> GetListByBeneficiaryAsync(Guid beneficiaryId)
        => GetListAsync(beneficiaryId);

    public async Task<BeneficiaryDocument?> GetByIdAsync(Guid id)
    {
        return await _db.Set<BeneficiaryDocument>()
            .Include(x => x.Beneficiary)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(BeneficiaryDocument entity)
    {
        await _db.Set<BeneficiaryDocument>().AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Set<BeneficiaryDocument>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return;

        _db.Set<BeneficiaryDocument>().Remove(entity);
        await _db.SaveChangesAsync();
    }
}
