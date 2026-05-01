using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InfrastructureManagmentServices.Charity.Workflow
{
    /// <summary>
    /// يُنفَّذ بعد اكتمال أو رفض مسار الـ Workflow.
    /// تم تحسينه حتى لا يحمّل Collections كاملة عند الضغط على موافقة.
    /// </summary>
    public class WorkflowCompletionHandler : IWorkflowCompletionHandler
    {
        private readonly AppDbContext  _db;
        private readonly IMemoryCache  _cache;

        public WorkflowCompletionHandler(AppDbContext db, IMemoryCache cache)
        {
            _db    = db;
            _cache = cache;
        }

        public async Task OnCompletedAsync(
            string entityType, Guid entityId,
            string actorUserId, CancellationToken ct = default)
        {
            switch (entityType)
            {
                case "AidRequest":
                    await UpdateStatusAsync<BeneficiaryAidRequest>(entityId, "Approved", ct);
                    break;

                case "HumanitarianResearch":
                    await MarkHumanitarianResearchWorkflowApprovedAsync(entityId, actorUserId, ct);
                    break;

                case "ProjectProposal":
                    await UpdateProjectProposalStatusAsync(entityId, "Approved", ct);
                    break;

                case "AidCycle":
                    await UpdateStatusAsync<AidCycle>(entityId, "Approved", ct);
                    break;

                case "KafalaCase":
                    await UpdateKafalaCaseStatusAsync(entityId, "Active", ct);
                    break;
            }
        }

        public async Task OnRejectedAsync(
            string entityType, Guid entityId,
            string actorUserId, string? reason,
            CancellationToken ct = default)
        {
            switch (entityType)
            {
                case "AidRequest":
                    await UpdateStatusAsync<BeneficiaryAidRequest>(entityId, "Rejected", ct);
                    break;

                case "HumanitarianResearch":
                    await MarkHumanitarianResearchWorkflowRejectedAsync(entityId, actorUserId, reason, ct);
                    break;

                case "ProjectProposal":
                    await UpdateProjectProposalStatusAsync(entityId, "Rejected", ct);
                    break;

                case "AidCycle":
                    await UpdateStatusAsync<AidCycle>(entityId, "Cancelled", ct);
                    break;

                case "KafalaCase":
                    await UpdateKafalaCaseStatusAsync(entityId, "Closed", ct);
                    break;
            }
        }

        private async Task MarkHumanitarianResearchWorkflowApprovedAsync(
            Guid id, string actorUserId, CancellationToken ct)
        {
            var entity = await _db.Set<BeneficiaryHumanitarianResearch>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity == null)
                return;

            var now = DateTime.UtcNow;
            var userId = string.IsNullOrWhiteSpace(actorUserId) ? "system" : actorUserId;

            var hasWorkflowApprovalReview = await _db.Set<BeneficiaryHumanitarianResearchReview>()
                .AsNoTracking()
                .AnyAsync(x => x.ResearchId == id
                            && x.Decision == "Approve"
                            && (((x.Reason ?? string.Empty).Contains("سير العمل"))
                                || ((x.Notes ?? string.Empty).Contains("سير العمل"))), ct);

            if (!hasWorkflowApprovalReview)
            {
                await _db.Set<BeneficiaryHumanitarianResearchReview>().AddAsync(new BeneficiaryHumanitarianResearchReview
                {
                    Id = Guid.NewGuid(),
                    ResearchId = entity.Id,
                    ReviewerUserId = userId,
                    ReviewedByUserId = userId,
                    Decision = "Approve",
                    Reason = "تم اعتماد استمارة البحث الإنساني من مسار سير العمل",
                    Notes = "تم تسجيل هذه المراجعة تلقائيًا بعد اكتمال جميع موافقات سير العمل.",
                    ReviewDateUtc = now,
                    ReviewDate = now,
                    ReviewerNotes = "مراجعة تلقائية من Workflow"
                }, ct);
            }

            var hasCommitteeDecision = await _db.Set<BeneficiaryHumanitarianResearchCommitteeEvaluation>()
                .AsNoTracking()
                .AnyAsync(x => x.ResearchId == id, ct);

            entity.ReviewedAtUtc = now;
            entity.ReviewedByUserId = userId;
            entity.ReviewDecision = "Approve";
            entity.ReviewReason = "تم اعتماد استمارة البحث الإنساني من مسار سير العمل";

            if (hasCommitteeDecision)
            {
                entity.Status = "CommitteeDecided";
                entity.CommitteeDecidedAtUtc ??= now;
            }
            else
            {
                entity.Status = "SentToCommittee";
                entity.SentToCommitteeAtUtc ??= now;
                entity.CommitteeSentAtUtc ??= now;
                entity.CommitteeSentByUserId ??= userId;
                entity.CommitteeDecidedAtUtc = null;
                entity.CommitteeDecisionAtUtc = null;
                entity.CommitteeDecision = null;
                entity.CommitteeDecisionNotes = null;
            }

            entity.UpdatedAtUtc = now;
            await _db.SaveChangesAsync(ct);
        }

        private async Task MarkHumanitarianResearchWorkflowRejectedAsync(
            Guid id, string actorUserId, string? reason, CancellationToken ct)
        {
            var entity = await _db.Set<BeneficiaryHumanitarianResearch>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity == null)
                return;

            var now = DateTime.UtcNow;
            var userId = string.IsNullOrWhiteSpace(actorUserId) ? "system" : actorUserId;
            var rejectReason = string.IsNullOrWhiteSpace(reason)
                ? "تم رفض استمارة البحث الإنساني من مسار سير العمل"
                : reason.Trim();

            var hasWorkflowRejectReview = await _db.Set<BeneficiaryHumanitarianResearchReview>()
                .AsNoTracking()
                .AnyAsync(x => x.ResearchId == id
                            && x.Decision == "Reject"
                            && (((x.Reason ?? string.Empty).Contains("سير العمل"))
                                || ((x.Notes ?? string.Empty).Contains("سير العمل"))), ct);

            if (!hasWorkflowRejectReview)
            {
                await _db.Set<BeneficiaryHumanitarianResearchReview>().AddAsync(new BeneficiaryHumanitarianResearchReview
                {
                    Id = Guid.NewGuid(),
                    ResearchId = entity.Id,
                    ReviewerUserId = userId,
                    ReviewedByUserId = userId,
                    Decision = "Reject",
                    Reason = rejectReason,
                    Notes = "تم تسجيل هذه المراجعة تلقائيًا بعد رفض مسار سير العمل.",
                    ReviewDateUtc = now,
                    ReviewDate = now,
                    ReviewerNotes = "رفض تلقائي من Workflow"
                }, ct);
            }

            entity.Status = "ReviewedRejected";
            entity.ReviewedAtUtc = now;
            entity.ReviewedByUserId = userId;
            entity.ReviewDecision = "Reject";
            entity.ReviewReason = rejectReason;
            entity.UpdatedAtUtc = now;
            await _db.SaveChangesAsync(ct);
        }

        private async Task UpdateStatusAsync<T>(Guid id, string newStatus, CancellationToken ct) where T : class
        {
            var entity = await _db.Set<T>().FindAsync(new object[] { id }, ct);
            if (entity == null) return;

            var prop = typeof(T).GetProperty("Status");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(entity, newStatus);
                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task UpdateProjectProposalStatusAsync(Guid id, string newStatus, CancellationToken ct)
        {
            await _db.Set<ProjectProposal>()
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);

            // مسح الـ Cache عشان الـ Details تحمّل البيانات المحدّثة فوراً
            _cache.Remove($"proposal_{id}");
        }

        private async Task UpdateKafalaCaseStatusAsync(Guid id, string newStatus, CancellationToken ct)
        {
            var entity = await _db.Set<KafalaCase>().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return;
            entity.Status = newStatus;
            await _db.SaveChangesAsync(ct);
        }
    }

    public interface IWorkflowCompletionHandler
    {
        Task OnCompletedAsync(string entityType, Guid entityId,
            string actorUserId, CancellationToken ct = default);

        Task OnRejectedAsync(string entityType, Guid entityId,
            string actorUserId, string? reason, CancellationToken ct = default);
    }
}
