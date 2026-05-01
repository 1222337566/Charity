using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfrastructureManagmentDataAccess.Repositories.Charity;

public class BeneficiaryHumanitarianResearchRepository : IBeneficiaryHumanitarianResearchRepository
{
    private readonly AppDbContext _db;

    public BeneficiaryHumanitarianResearchRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BeneficiaryHumanitarianResearch>> GetAllAsync()
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Beneficiary)
            .AsNoTracking()
            .OrderByDescending(x => x.RequestDate)
            .ToListAsync();
    }
    public async Task<bool> CodeExistsAsync(string code)
            => await _db.Set<BeneficiaryHumanitarianResearch>().AnyAsync(x => x.ResearchNumber == code);
    public async Task<BeneficiaryHumanitarianResearch?> GetByIdAsync(Guid id)
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BeneficiaryHumanitarianResearch?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Beneficiary)
            .Include(x => x.Reviews)
            .Include(x => x.CommitteeEvaluations)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BeneficiaryHumanitarianResearch?> GetByIdForWorkflowAsync(Guid id)
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Reviews)
            .Include(x => x.CommitteeEvaluations)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ResearchNumberExistsAsync(string researchNumber)
    {
        if (string.IsNullOrWhiteSpace(researchNumber))
            return false;

        var normalized = researchNumber.Trim();
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .AnyAsync(x => x.ResearchNumber == normalized);
    }

    public async Task AddAsync(BeneficiaryHumanitarianResearch entity)
    {
        await _db.Set<BeneficiaryHumanitarianResearch>().AddAsync(entity);
    }

    public async Task AddCommitteeEvaluationAsync(BeneficiaryHumanitarianResearchCommitteeEvaluation entity)
    {
        await _db.Set<BeneficiaryHumanitarianResearchCommitteeEvaluation>().AddAsync(entity);
    }

    public async Task UpdateBeneficiaryStatusAsync(Guid beneficiaryId, Guid statusId)
    {
        if (beneficiaryId == Guid.Empty)
            return;

        await _db.Set<Beneficiary>()
            .Where(x => x.Id == beneficiaryId && (!x.StatusId.HasValue || x.StatusId.Value != statusId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.StatusId, statusId)
                .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow));
    }

    public Task UpdateAsync(BeneficiaryHumanitarianResearch entity)
    {
        _db.Set<BeneficiaryHumanitarianResearch>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BeneficiaryHumanitarianResearchListItemVm>> GetListAsync()
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Beneficiary)
            .AsNoTracking()
            .OrderByDescending(x => x.RequestDate)
            .Select(x => new BeneficiaryHumanitarianResearchListItemVm
            {
                Id = x.Id,
                BeneficiaryId = x.BeneficiaryId,
                BeneficiaryCode = x.Beneficiary != null ? x.Beneficiary.Code : null,
                ResearchNumber = x.ResearchNumber,
                FullName = x.FullName,
                ApplicantName = x.ApplicantName,
                RequestDate = x.RequestDate,
                Status = x.Status,
                PriorityLevel = x.PriorityLevel
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BeneficiaryHumanitarianResearchListItemVm>> GetListByBeneficiaryAsync(Guid beneficiaryId)
    {
        return await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Beneficiary)
            .Where(x => x.BeneficiaryId == beneficiaryId)
            .AsNoTracking()
            .OrderByDescending(x => x.RequestDate)
            .Select(x => new BeneficiaryHumanitarianResearchListItemVm
            {
                Id = x.Id,
                BeneficiaryId = x.BeneficiaryId,
                BeneficiaryCode = x.Beneficiary != null ? x.Beneficiary.Code : null,
                ResearchNumber = x.ResearchNumber,
                FullName = x.FullName,
                ApplicantName = x.ApplicantName,
                RequestDate = x.RequestDate,
                Status = x.Status,
                PriorityLevel = x.PriorityLevel
            })
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(CreateBeneficiaryHumanitarianResearchVm vm, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        var entity = new BeneficiaryHumanitarianResearch
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            ResearchNumber = (vm.ResearchNumber ?? string.Empty).Trim(),
            RequestDate = vm.RequestDate,
            ResearchDate = vm.ResearchDate,
            AidTypeName = vm.AidTypeName?.Trim(),
            ApplicantName = vm.ApplicantName.Trim(),
            SourceOfRequest = vm.SourceOfRequest?.Trim(),
            ResearcherCode = vm.ResearcherCode?.Trim(),
            ResearcherName = vm.ResearcherName?.Trim(),
            CommitteeCode = vm.CommitteeCode?.Trim(),
            PriorityLevel = vm.PriorityLevel?.Trim(),
            FullName = vm.FullName.Trim(),
            NickName = vm.NickName?.Trim(),
            NationalId = vm.NationalId?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            FamilyMembersCount = vm.FamilyMembersCount,
            MaritalStatus = vm.MaritalStatus?.Trim(),
            Age = vm.Age,
            TotalIncome = vm.TotalIncome,
            TotalExpenses = vm.TotalExpenses,
            AverageIncome = vm.AverageIncome,
            HasExistingProject = vm.HasExistingProject,
            ExistingProjectType = vm.ExistingProjectType?.Trim(),
            ExistingProjectSize = vm.ExistingProjectSize?.Trim(),
            RequiredNeedsPrimary = vm.RequiredNeedsPrimary?.Trim(),
            RequiredNeedsSecondary = vm.RequiredNeedsSecondary?.Trim(),
            HousingDescription = vm.HousingDescription?.Trim(),
            ResearcherReport = vm.ResearcherReport?.Trim(),
            ResearchManagerOpinion = vm.ResearchManagerOpinion?.Trim(),
            Status = "Draft",
            SubmittedByUserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _db.Set<BeneficiaryHumanitarianResearch>().AddAsync(entity);

        // بمجرد إنشاء/بدء استمارة البحث الإنساني يتحول ملف المستفيد إلى "تحت الدراسة".
        var beneficiary = await _db.Set<Beneficiary>().FirstOrDefaultAsync(x => x.Id == vm.BeneficiaryId);
        if (beneficiary != null)
        {
            beneficiary.StatusId = CharityLookupSeedIds.BeneficiaryStatusUnderReview;
            beneficiary.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return entity.Id;
    }
}
