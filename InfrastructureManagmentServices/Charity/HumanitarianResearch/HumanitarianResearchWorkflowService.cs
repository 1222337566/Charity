using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.HumanitarianResearch;

namespace InfrastructureManagmentServices.Charity.HumanitarianResearch;

public class HumanitarianResearchWorkflowService : IHumanitarianResearchWorkflowService
{
    private readonly IBeneficiaryHumanitarianResearchRepository _repository;

    public HumanitarianResearchWorkflowService(IBeneficiaryHumanitarianResearchRepository repository)
    {
        _repository = repository;
    }

  
    public async Task SubmitForReviewAsync(Guid researchId, string submittedByUserId)
    {
        var entity = await _repository.GetByIdAsync(researchId)
            ?? throw new InvalidOperationException("الاستمارة غير موجودة");

        if (entity.Status is not ("Draft" or "ReturnedForRevision"))
            throw new InvalidOperationException("لا يمكن إرسال الاستمارة إلا إذا كانت في حالة مسودة أو مُرجَعة للتعديل");

        entity.Status = "SubmittedForReview";
        entity.SubmittedAtUtc = DateTime.UtcNow;
        entity.SubmittedByUserId = submittedByUserId;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        // عند بدء مسار المراجعة تصبح حالة ملف المستفيد "تحت الدراسة".
        await _repository.UpdateBeneficiaryStatusAsync(
            entity.BeneficiaryId,
            CharityLookupSeedIds.BeneficiaryStatusUnderReview);

        await _repository.UpdateAsync(entity);
        await _repository.SaveAsync();
    }

  
    public async Task SendToCommitteeAsync(Guid researchId, string userId)
    {
        var entity = await _repository.GetByIdAsync(researchId)
            ?? throw new InvalidOperationException("الاستمارة غير موجودة");

        if (entity.Status != "ReviewedApproved")
            throw new InvalidOperationException("لا يمكن إحالة الاستمارة للجنة قبل اعتماد المراجع");

        entity.Status = "SentToCommittee";
        entity.SentToCommitteeAtUtc = DateTime.UtcNow;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        // تظل حالة المستفيد تحت الدراسة حتى صدور قرار اللجنة النهائي.
        await _repository.UpdateBeneficiaryStatusAsync(
            entity.BeneficiaryId,
            CharityLookupSeedIds.BeneficiaryStatusUnderReview);

        await _repository.UpdateAsync(entity);
        await _repository.SaveAsync();
    }

    public async Task CommitteeDecisionAsync(
        Guid researchId,
        DateTime meetingDate,
        string decision,
        string? aidType,
        decimal? approvedAmount,
        int? durationMonths,
        string? notes,
        string? approvedByUserId)
    {
        // لا نستخدم GetByIdForWorkflowAsync هنا لأنه يحمل CommitteeEvaluations،
        // ثم Update(entity) قد يعلّم الـ graph بالكامل Modified، فيحاول EF عمل UPDATE
        // لقرار لجنة جديد لم يُدرج بعد، وينتج DbUpdateConcurrencyException.
        var entity = await _repository.GetByIdAsync(researchId)
            ?? throw new InvalidOperationException("الاستمارة غير موجودة");

        if (entity.Status != "SentToCommittee")
            throw new InvalidOperationException("لا يمكن تسجيل قرار اللجنة قبل إحالة الاستمارة إليها");

        var committeeEvaluation = new BeneficiaryHumanitarianResearchCommitteeEvaluation
        {
            Id = Guid.NewGuid(),
            ResearchId = entity.Id,
            CommitteeMeetingDate = meetingDate,
            Decision = decision,
            ApprovedAidType = aidType,
            ApprovedAmount = approvedAmount,
            DurationMonths = durationMonths,
            Notes = notes,
            ApprovedByUserId = approvedByUserId
        };

        await _repository.AddCommitteeEvaluationAsync(committeeEvaluation);

        entity.Status = "CommitteeDecided";
        entity.CommitteeDecidedAtUtc = DateTime.UtcNow;
        entity.CommitteeDecisionAtUtc = DateTime.UtcNow;
        entity.CommitteeDecisionByUserId = approvedByUserId;
        entity.CommitteeDecision = decision;
        entity.CommitteeDecisionNotes = notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        var beneficiaryStatusId = ResolveBeneficiaryStatusAfterCommitteeDecision(decision);
        await _repository.UpdateBeneficiaryStatusAsync(entity.BeneficiaryId, beneficiaryStatusId);

        await _repository.SaveAsync();
    }
    public async Task ReviewAsync(Guid researchId, string reviewerUserId, string decision, string reason, string? notes)
    {
        var research = await _repository.GetByIdForWorkflowAsync(researchId);
        if (research is null)
            throw new InvalidOperationException("Humanitarian research was not found.");

        if (research.Status is not ("SubmittedForReview" or "ReturnedForRevision"))
            throw new InvalidOperationException("Research is not currently pending review.");

        var review = new BeneficiaryHumanitarianResearchReview
        {
            Id = Guid.NewGuid(),
           ResearchId= research.Id,
            ReviewerUserId = reviewerUserId,
            Decision = decision,
            Reason = reason,
            Notes = notes,
            ReviewDateUtc = DateTime.UtcNow
        };

        research.Reviews ??= new List<BeneficiaryHumanitarianResearchReview>();
        research.Reviews.Add(review);

        research.ReviewedAtUtc = DateTime.UtcNow;
        research.ReviewedByUserId = reviewerUserId;

        switch (decision)
        {
            case "Approve":
                research.Status = "ReviewedApproved";
                break;
            case "Reject":
                research.Status = "ReviewedRejected";
                break;
            case "ReturnForRevision":
                research.Status = "ReturnedForRevision";
                break;
            default:
                throw new InvalidOperationException("Invalid review decision.");
        }

        research.ReviewDecision = decision;
        research.ReviewReason = reason;

        // بمجرد بدء/تنفيذ المراجعة يظل ملف المستفيد في حالة "تحت الدراسة".
        await _repository.UpdateBeneficiaryStatusAsync(
            research.BeneficiaryId,
            CharityLookupSeedIds.BeneficiaryStatusUnderReview);

        await _repository.SaveAsync();
    }

    private static Guid ResolveBeneficiaryStatusAfterCommitteeDecision(string? decision)
    {
        var value = (decision ?? string.Empty).Trim();

        if (string.Equals(value, "RejectAid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "Reject", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "Rejected", StringComparison.OrdinalIgnoreCase)
            || value.Contains("رفض", StringComparison.OrdinalIgnoreCase)
            || value.Contains("مرفوض", StringComparison.OrdinalIgnoreCase))
            return CharityLookupSeedIds.BeneficiaryStatusRejected;

        if (string.Equals(value, "Suspend", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "Suspended", StringComparison.OrdinalIgnoreCase)
            || value.Contains("تعليق", StringComparison.OrdinalIgnoreCase)
            || value.Contains("موقوف", StringComparison.OrdinalIgnoreCase))
            return CharityLookupSeedIds.BeneficiaryStatusSuspended;

        // أي قرار لجنة غير الرفض/التعليق يعتبر اعتمادًا لملف المستفيد.
        return CharityLookupSeedIds.BeneficiaryStatusApproved;
    }



  
}
