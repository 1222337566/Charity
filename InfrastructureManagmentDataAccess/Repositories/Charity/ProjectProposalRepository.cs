using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.ProjectProposals;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    /// <summary>
    /// يستخدم IDbContextFactory لإنشاء DbContext مستقل لكل query
    /// مما يتيح Task.WhenAll الآمن على remote DB
    /// </summary>
    public class ProjectProposalRepository : IProjectProposalRepository
    {
        private readonly AppDbContext _db;      // للعمليات العادية
        private readonly IDbContextFactory<AppDbContext> _factory; // للـ parallel reads
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
        private static string CacheKey(Guid id) => $"proposal_{id}";

        public ProjectProposalRepository(
            AppDbContext db,
            IDbContextFactory<AppDbContext> factory,
            IMemoryCache cache)
        {
            _db = db;
            _factory = factory;
            _cache = cache;
        }

        public async Task<List<ProjectProposal>> GetAllAsync()
        {
            return await _db.Set<ProjectProposal>()
                .AsNoTracking()
                .OrderByDescending(x => x.SubmissionDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<ProjectProposal?> GetByIdAsync(Guid id)
        {
            return await _db.Set<ProjectProposal>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProjectProposal?> GetByIdWithDetailsAsync(Guid id)
        {
            // ── Cache hit → فوري ──
            if (_cache.TryGetValue(CacheKey(id), out ProjectProposal? cached))
                return cached;

            // ── Cache miss → كل query على DbContext مستقل → Task.WhenAll آمن ──
            // كل واحدة تفتح connection منفصلة وتشتغل بالتوازي
            // بدل N × 200ms = ~200ms فقط (وقت الأبطأ واحدة)

            await using var ctx0 = await _factory.CreateDbContextAsync();
            await using var ctx1 = await _factory.CreateDbContextAsync();
            await using var ctx2 = await _factory.CreateDbContextAsync();
            await using var ctx3 = await _factory.CreateDbContextAsync();
            await using var ctx4 = await _factory.CreateDbContextAsync();
            await using var ctx5 = await _factory.CreateDbContextAsync();
            await using var ctx6 = await _factory.CreateDbContextAsync();
            await using var ctx7 = await _factory.CreateDbContextAsync();
            await using var ctx8 = await _factory.CreateDbContextAsync();
            await using var ctx9 = await _factory.CreateDbContextAsync();
            await using var ctx10 = await _factory.CreateDbContextAsync();

            var t0 = ctx0.Set<ProjectProposal>().AsNoTracking()
                         .FirstOrDefaultAsync(x => x.Id == id);

            var t1 = ctx1.Set<ProjectProposalPastExperience>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t2 = ctx2.Set<ProjectProposalTargetGroup>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t3 = ctx3.Set<ProjectProposalObjective>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t4 = ctx4.Set<ProjectProposalActivity>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id)
                         .OrderBy(x => x.SortOrder).ToListAsync();

            var t5 = ctx5.Set<ProjectProposalWorkPlan>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t6 = ctx6.Set<ProjectProposalMonitoringIndicator>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t7 = ctx7.Set<ProjectProposalTeamMember>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id).ToListAsync();

            var t8 = ctx8.Set<ProjectProposalAttachment>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id)
                         .Select(x => new ProjectProposalAttachment
                         {
                             Id = x.Id,
                             ProjectProposalId = x.ProjectProposalId,
                             OriginalFileName = x.OriginalFileName,
                             ContentType = x.ContentType,
                             FileExtension = x.FileExtension,
                             FileSizeBytes = x.FileSizeBytes,
                             AttachmentType = x.AttachmentType,
                             Notes = x.Notes,
                             CreatedByUserId = x.CreatedByUserId,
                             CreatedAtUtc = x.CreatedAtUtc,
                             FileContent = Array.Empty<byte>()
                         }).ToListAsync();

            var t9 = ctx9.Set<ProjectProposalPhase>().AsNoTracking()
                         .Where(x => x.ProjectProposalId == id)
                         .OrderBy(x => x.SortOrder).ToListAsync();

            var t10 = ctx10.Set<ProjectProposalPhaseActivity>().AsNoTracking()
                           .Where(x => x.Phase!.ProjectProposalId == id)
                           .OrderBy(x => x.SortOrder).ToListAsync();

            // كلهم يبدأوا مع بعض — ننتظر الأبطأ واحدة فقط
            await Task.WhenAll(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);

            var entity = await t0;
            if (entity == null) return null;

            // تسكين أنشطة المراحل
            var phases = await t9;
            var phaseActs = await t10;
            var actsByPhase = phaseActs.GroupBy(a => a.PhaseId)
                                       .ToDictionary(g => g.Key, g => g.ToList());
            foreach (var ph in phases)
                ph.Activities = actsByPhase.TryGetValue(ph.Id, out var a) ? a : new();

            entity.PastExperiences = await t1;
            entity.TargetGroups = await t2;
            entity.Objectives = await t3;
            entity.Activities = await t4;
            entity.WorkPlanItems = await t5;
            entity.MonitoringIndicators = await t6;
            entity.TeamMembers = await t7;
            entity.Attachments = await t8;
            entity.Phases = phases;

            _cache.Set(CacheKey(id), entity, CacheTtl);
            return entity;
        }

        public async Task AddAsync(ProjectProposal entity)
        {
            _db.Set<ProjectProposal>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectProposal entity)
        {
            var id = entity.Id;
            _cache.Remove(CacheKey(id));

            // حذف تسلسلي بسبب FK
            await _db.Set<ProjectProposalPhaseActivity>()
                .Where(x => x.Phase!.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalPhase>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalPastExperience>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalTargetGroup>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalObjective>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalActivity>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalWorkPlan>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalMonitoringIndicator>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();
            await _db.Set<ProjectProposalTeamMember>()
                .Where(x => x.ProjectProposalId == id).ExecuteDeleteAsync();

            _db.Entry(entity).State = EntityState.Modified;

            foreach (var x in entity.PastExperiences) x.ProjectProposalId = id;
            foreach (var x in entity.TargetGroups) x.ProjectProposalId = id;
            foreach (var x in entity.Objectives) x.ProjectProposalId = id;
            foreach (var x in entity.Activities) x.ProjectProposalId = id;
            foreach (var x in entity.WorkPlanItems) x.ProjectProposalId = id;
            foreach (var x in entity.MonitoringIndicators) x.ProjectProposalId = id;
            foreach (var x in entity.TeamMembers) x.ProjectProposalId = id;
            foreach (var ph in entity.Phases)
            {
                ph.ProjectProposalId = id;
                foreach (var pa in ph.Activities) pa.PhaseId = ph.Id;
            }

            if (entity.PastExperiences.Any())
                _db.Set<ProjectProposalPastExperience>().AddRange(entity.PastExperiences);
            if (entity.TargetGroups.Any())
                _db.Set<ProjectProposalTargetGroup>().AddRange(entity.TargetGroups);
            if (entity.Objectives.Any())
                _db.Set<ProjectProposalObjective>().AddRange(entity.Objectives);
            if (entity.Activities.Any())
                _db.Set<ProjectProposalActivity>().AddRange(entity.Activities);
            if (entity.WorkPlanItems.Any())
                _db.Set<ProjectProposalWorkPlan>().AddRange(entity.WorkPlanItems);
            if (entity.MonitoringIndicators.Any())
                _db.Set<ProjectProposalMonitoringIndicator>().AddRange(entity.MonitoringIndicators);
            if (entity.TeamMembers.Any())
                _db.Set<ProjectProposalTeamMember>().AddRange(entity.TeamMembers);
            if (entity.Phases.Any())
                _db.Set<ProjectProposalPhase>().AddRange(entity.Phases);

            await _db.SaveChangesAsync();
        }

        public async Task<bool> ProposalNumberExistsAsync(string proposalNumber, Guid? excludeId = null)
        {
            return await _db.Set<ProjectProposal>()
                .AnyAsync(x => x.ProposalNumber == proposalNumber
                            && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task UpdateStatusAsync(Guid id, string status)
        {
            await _db.Set<ProjectProposal>()
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, status)
                    .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow));
            _cache.Remove(CacheKey(id));
        }

        public async Task<ProjectProposalAttachment?> GetAttachmentContentAsync(Guid attachmentId)
        {
            return await _db.Set<ProjectProposalAttachment>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == attachmentId);
        }
    }
}
