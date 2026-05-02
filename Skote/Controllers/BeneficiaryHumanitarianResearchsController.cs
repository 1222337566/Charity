using InfrastructureManagmentWebFramework.Models.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.HumanitarianResearch;
using InfrastructureManagmentServices.Charity.HumanitarianResearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Skote.Helpers;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using InfrastructureManagmentServices.Charity.Workflow;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InfrastructureManagmentCore.Domains.Identity;

namespace Skote.Controllers;

[Authorize(Policy = CharityPolicies.BeneficiariesView)]
public class BeneficiaryHumanitarianResearchsController : Controller
{
    private readonly IBeneficiaryHumanitarianResearchRepository _repository;
    private readonly IHumanitarianResearchWorkflowService _workflowService;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly IWorkflowService _workflowService1;
    private readonly IUserActivityService _activityService;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BeneficiaryHumanitarianResearchsController(
        IBeneficiaryHumanitarianResearchRepository repository,
        IHumanitarianResearchWorkflowService workflowService,
        IBeneficiaryRepository beneficiaryRepository,
        IWorkflowService workflowService1,
        IUserActivityService activityService,
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _workflowService1 = workflowService1;
        _workflowService = workflowService;
        _beneficiaryRepository = beneficiaryRepository;
        _activityService = activityService;
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? beneficiaryId)
    {
        IReadOnlyList<BeneficiaryHumanitarianResearchListItemVm> model;

        if (beneficiaryId.HasValue)
        {
            model = await _repository.GetListByBeneficiaryAsync(beneficiaryId.Value);
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId.Value);
            ViewBag.BeneficiaryId = beneficiaryId.Value;
            ViewBag.BeneficiaryName = beneficiary?.FullName ?? string.Empty;
        }
        else
        {
            model = await _repository.GetListAsync();
        }

        return View(model);
    }

    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid? beneficiaryId)
    {
        var currentUserDisplayName = await GetCurrentUserDisplayNameAsync();

        var vm = new CreateBeneficiaryHumanitarianResearchVm()
        {
            ResearchNumber = await GenerateHumanCodeAsync(),
            ResearcherCode = await GenerateResearcherCodeAsync(),
            CommitteeCode = await GenerateCommitteeCodeAsync(),
            ApplicantName = currentUserDisplayName,
            ResearcherName = currentUserDisplayName,
            ResearchDate = DateTime.Today,
            RequestDate = DateTime.Today
        };

        await FillBeneficiariesAsync(vm);
        await PrefillFromBeneficiaryAsync(vm, beneficiaryId);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryHumanitarianResearchVm vm)
    {
        var currentUserDisplayName = await GetCurrentUserDisplayNameAsync();

        vm.ResearchNumber = string.IsNullOrWhiteSpace(vm.ResearchNumber)
            ? await GenerateHumanCodeAsync()
            : vm.ResearchNumber.Trim();

        vm.ResearcherCode = string.IsNullOrWhiteSpace(vm.ResearcherCode)
            ? await GenerateResearcherCodeAsync()
            : vm.ResearcherCode.Trim();

        vm.CommitteeCode = string.IsNullOrWhiteSpace(vm.CommitteeCode)
            ? await GenerateCommitteeCodeAsync()
            : vm.CommitteeCode.Trim();

        vm.ApplicantName = currentUserDisplayName;

        if (string.IsNullOrWhiteSpace(vm.ResearcherName))
            vm.ResearcherName = currentUserDisplayName;

        ModelState.Remove(nameof(vm.ResearchNumber));
        ModelState.Remove(nameof(vm.ResearcherCode));
        ModelState.Remove(nameof(vm.CommitteeCode));
        ModelState.Remove(nameof(vm.ApplicantName));
        ModelState.Remove(nameof(vm.ResearcherName));

        if (!ModelState.IsValid)
        {
            await FillBeneficiariesAsync(vm);
            return View(vm);
        }

        while (await _repository.ResearchNumberExistsAsync(vm.ResearchNumber))
            vm.ResearchNumber = await GenerateHumanCodeAsync();

        while (await _db.Set<BeneficiaryHumanitarianResearch>()
            .AsNoTracking()
            .AnyAsync(x => x.ResearcherCode == vm.ResearcherCode))
            vm.ResearcherCode = await GenerateResearcherCodeAsync();

        while (await _db.Set<BeneficiaryHumanitarianResearch>()
            .AsNoTracking()
            .AnyAsync(x => x.CommitteeCode == vm.CommitteeCode))
            vm.CommitteeCode = await GenerateCommitteeCodeAsync();

        var entityId = await _repository.CreateAsync(vm, User);

        // ── ربط مسار الموافقة العام (لوحة سير العمل) ──
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await _workflowService1.InitiateAsync(
            entityType:        "HumanitarianResearch",
            entityId:          entityId,
            entityTitle:       $"بحث إنساني — {vm.ApplicantName} ({vm.ResearchNumber})",
            submittedByUserId: userId
        );

        return RedirectToAction(nameof(Index), new { beneficiaryId = vm.BeneficiaryId });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        await SyncHumanitarianResearchWorkflowStateAsync(id);

        var entity = await _repository.GetByIdWithDetailsAsync(id);
        if (entity == null)
            return NotFound();

        return View(entity);
    }

    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitForReview(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await _workflowService.SubmitForReviewAsync(id, userId);

        await _activityService.LogBusinessAsync(
            userId,
            UserActivityBusinessActions.ResearchSubmittedForReview,
            $"تم إرسال الاستمارة رقم {entity.ResearchNumber} للمراجعة",
            entityName: "الاستمارة الإنسانية",
            entityId: entity.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["ResearchNumber"] = entity.ResearchNumber,
                ["BeneficiaryId"] = entity.BeneficiaryId.ToString(),
                ["ApplicantName"] = entity.ApplicantName,
                ["Status"] = "SubmittedForReview"
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم إرسال الاستمارة إلى المراجع";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.ResearchReview)]
    [HttpGet]
    public async Task<IActionResult> Review(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new ReviewBeneficiaryHumanitarianResearchVm
        {
            Id = id,
            Decisions = BuildReviewDecisions()
        };

        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.ResearchReview)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(ReviewBeneficiaryHumanitarianResearchVm vm)
    {
        vm.Decisions = BuildReviewDecisions();
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await _workflowService.ReviewAsync(
            vm.Id,
            userId,
            vm.Decision,
            vm.Reason,
            vm.Notes);

        var nextStatus = vm.Decision switch
        {
            "Approve" => "ReviewedApproved",
            "Reject" => "ReviewedRejected",
            "ReturnForRevision" => "ReturnedForRevision",
            _ => vm.Decision
        };

        await _activityService.LogBusinessAsync(
            userId,
            UserActivityBusinessActions.ResearchReviewed,
            $"تمت مراجعة الاستمارة رقم {entity.ResearchNumber} بقرار {vm.Decision}",
            entityName: "الاستمارة الإنسانية",
            entityId: vm.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["ResearchNumber"] = entity.ResearchNumber,
                ["Decision"] = vm.Decision,
                ["Reason"] = vm.Reason,
                ["Notes"] = vm.Notes,
                ["Status"] = nextStatus
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم تسجيل مراجعة الاستمارة";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [Authorize(Policy = CharityPolicies.ResearchReview)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToCommittee(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await _workflowService.SendToCommitteeAsync(id, userId);

        await _activityService.LogBusinessAsync(
            userId,
            UserActivityBusinessActions.ResearchSentToCommittee,
            $"تمت إحالة الاستمارة رقم {entity.ResearchNumber} إلى اللجنة",
            entityName: "الاستمارة الإنسانية",
            entityId: id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["ResearchNumber"] = entity.ResearchNumber,
                ["Status"] = "SentToCommittee"
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تمت إحالة الاستمارة إلى اللجنة";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpGet]
    public async Task<IActionResult> Committee(Guid id)
    {
        await SyncHumanitarianResearchWorkflowStateAsync(id);

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new CommitteeBeneficiaryHumanitarianResearchVm
        {
            Id = id,
            CommitteeMeetingDate = DateTime.Today,
            Decisions = BuildCommitteeDecisions()
        };

        await FillCommitteeAidTypesAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Committee(CommitteeBeneficiaryHumanitarianResearchVm vm)
    {
        vm.Decisions = BuildCommitteeDecisions();
        await FillCommitteeAidTypesAsync(vm);

        var isApprovalDecision = IsCommitteeApprovalDecision(vm.Decision);
        AidTypeLookup? selectedAidType = null;

        if (isApprovalDecision)
        {
            if (!vm.ApprovedAidTypeId.HasValue || vm.ApprovedAidTypeId.Value == Guid.Empty)
            {
                ModelState.AddModelError(nameof(vm.ApprovedAidTypeId), "نوع المساعدة مطلوب عند الموافقة على المساعدة.");
            }
            else
            {
                selectedAidType = await _db.Set<AidTypeLookup>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == vm.ApprovedAidTypeId.Value && x.IsActive);

                if (selectedAidType == null)
                    ModelState.AddModelError(nameof(vm.ApprovedAidTypeId), "نوع المساعدة المحدد غير موجود أو غير مفعل.");
                else
                    vm.ApprovedAidType = selectedAidType.NameAr;
            }

            if (!vm.ApprovedAmount.HasValue || vm.ApprovedAmount.Value <= 0)
                ModelState.AddModelError(nameof(vm.ApprovedAmount), "القيمة المعتمدة يجب أن تكون أكبر من صفر عند الموافقة.");
        }
        else
        {
            vm.ApprovedAidTypeId = null;
            vm.ApprovedAidType = null;
            vm.ApprovedAmount = null;
            vm.DurationMonths = null;
        }

        if (!ModelState.IsValid)
            return View(vm);

        await SyncHumanitarianResearchWorkflowStateAsync(vm.Id);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _workflowService.CommitteeDecisionAsync(
            vm.Id,
            vm.CommitteeMeetingDate,
            vm.Decision,
            vm.ApprovedAidType,
            vm.ApprovedAmount,
            vm.DurationMonths,
            vm.Notes,
            userId);

        await _activityService.LogBusinessAsync(
            userId,
            UserActivityBusinessActions.ResearchCommitteeDecision,
            $"تم تسجيل قرار اللجنة على الاستمارة رقم {entity.ResearchNumber}",
            entityName: "الاستمارة الإنسانية",
            entityId: vm.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["ResearchNumber"] = entity.ResearchNumber,
                ["Decision"] = vm.Decision,
                ["ApprovedAidTypeId"] = vm.ApprovedAidTypeId?.ToString(),
                ["ApprovedAidType"] = vm.ApprovedAidType,
                ["ApprovedAmount"] = vm.ApprovedAmount?.ToString("0.##"),
                ["DurationMonths"] = vm.DurationMonths?.ToString(),
                ["CommitteeMeetingDate"] = vm.CommitteeMeetingDate.ToString("yyyy-MM-dd"),
                ["Status"] = "CommitteeDecided"
            },
            ct: HttpContext.RequestAborted);

        if (isApprovalDecision && selectedAidType != null)
        {
            await EnsureApprovedAidRequestAndDisbursementFromCommitteeDecisionAsync(
                entity,
                selectedAidType,
                vm,
                userId);

            TempData["Success"] = "تم تسجيل قرار اللجنة، وإنشاء طلب مساعدة معتمد وسجل صرف متاح للتنفيذ.";
        }
        else
        {
            TempData["Success"] = "تم تسجيل قرار اللجنة";
        }

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }


    private async Task SyncHumanitarianResearchWorkflowStateAsync(Guid id)
    {
        var steps = await _db.Set<WorkflowStep>()
            .AsNoTracking()
            .Where(x => x.EntityType == "HumanitarianResearch" && x.EntityId == id && x.IsActive)
            .ToListAsync();

        if (steps.Count == 0)
            return;

        var workflowRejected = steps.Any(x => string.Equals(x.Status, "Rejected", StringComparison.OrdinalIgnoreCase));
        var workflowCompleted = steps.All(x => string.Equals(x.Status, "Approved", StringComparison.OrdinalIgnoreCase));

        if (!workflowRejected && !workflowCompleted)
            return;

        var entity = await _db.Set<BeneficiaryHumanitarianResearch>()
            .Include(x => x.Reviews)
            .Include(x => x.CommitteeEvaluations)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        var now = DateTime.UtcNow;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var changed = false;
        entity.Reviews ??= new List<BeneficiaryHumanitarianResearchReview>();

        if (workflowRejected)
        {
            var hasWorkflowRejectReview = entity.Reviews.Any(x =>
                string.Equals(x.Decision, "Reject", StringComparison.OrdinalIgnoreCase)
                && ((x.Reason ?? string.Empty).Contains("سير العمل")
                    || (x.Notes ?? string.Empty).Contains("سير العمل")));

            if (!hasWorkflowRejectReview)
            {
                entity.Reviews.Add(new BeneficiaryHumanitarianResearchReview
                {
                    Id = Guid.NewGuid(),
                    ResearchId = entity.Id,
                    ReviewerUserId = userId,
                    ReviewedByUserId = userId,
                    Decision = "Reject",
                    Reason = "تم رفض استمارة البحث الإنساني من مسار سير العمل",
                    Notes = "تم تسجيل هذه المراجعة تلقائيًا عند فتح تفاصيل الاستمارة.",
                    ReviewDateUtc = now,
                    ReviewDate = now,
                    ReviewerNotes = "Sync تلقائي من Workflow"
                });
                changed = true;
            }

            if (!string.Equals(entity.Status, "ReviewedRejected", StringComparison.OrdinalIgnoreCase))
            {
                entity.Status = "ReviewedRejected";
                changed = true;
            }

            entity.ReviewedAtUtc ??= now;
            entity.ReviewedByUserId ??= userId;
            entity.ReviewDecision = "Reject";
            entity.ReviewReason = "تم رفض استمارة البحث الإنساني من مسار سير العمل";
        }
        else if (workflowCompleted)
        {
            var hasWorkflowApprovalReview = entity.Reviews.Any(x =>
                string.Equals(x.Decision, "Approve", StringComparison.OrdinalIgnoreCase)
                && ((x.Reason ?? string.Empty).Contains("سير العمل")
                    || (x.Notes ?? string.Empty).Contains("سير العمل")));

            if (!hasWorkflowApprovalReview)
            {
                entity.Reviews.Add(new BeneficiaryHumanitarianResearchReview
                {
                    Id = Guid.NewGuid(),
                    ResearchId = entity.Id,
                    ReviewerUserId = userId,
                    ReviewedByUserId = userId,
                    Decision = "Approve",
                    Reason = "تم اعتماد استمارة البحث الإنساني من مسار سير العمل",
                    Notes = "تم تسجيل هذه المراجعة تلقائيًا عند فتح تفاصيل الاستمارة بعد اكتمال الموافقات.",
                    ReviewDateUtc = now,
                    ReviewDate = now,
                    ReviewerNotes = "Sync تلقائي من Workflow"
                });
                changed = true;
            }

            entity.ReviewedAtUtc ??= now;
            entity.ReviewedByUserId ??= userId;
            entity.ReviewDecision = "Approve";
            entity.ReviewReason = "تم اعتماد استمارة البحث الإنساني من مسار سير العمل";

            var hasCommitteeDecision = entity.CommitteeEvaluations?.Any() == true;
            var requiredStatus = hasCommitteeDecision ? "CommitteeDecided" : "SentToCommittee";

            if (!string.Equals(entity.Status, requiredStatus, StringComparison.OrdinalIgnoreCase))
            {
                entity.Status = requiredStatus;
                changed = true;
            }

            if (!hasCommitteeDecision)
            {
                entity.SentToCommitteeAtUtc ??= now;
                entity.CommitteeSentAtUtc ??= now;
                entity.CommitteeSentByUserId ??= userId;
                entity.CommitteeDecidedAtUtc = null;
                entity.CommitteeDecisionAtUtc = null;
                entity.CommitteeDecision = null;
                entity.CommitteeDecisionNotes = null;
                changed = true;
            }
        }

        if (changed)
        {
            entity.UpdatedAtUtc = now;
            await _db.SaveChangesAsync();
        }
    }

    private async Task FillBeneficiariesAsync(CreateBeneficiaryHumanitarianResearchVm vm)
    {
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        vm.Beneficiaries = beneficiaries
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(x.Code) ? x.FullName : $"{x.Code} - {x.FullName}"
            })
            .ToList();
    }

    private async Task PrefillFromBeneficiaryAsync(CreateBeneficiaryHumanitarianResearchVm vm, Guid? beneficiaryId)
    {
        if (!beneficiaryId.HasValue || beneficiaryId.Value == Guid.Empty)
            return;

        var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId.Value);
        if (beneficiary == null)
            return;

        vm.BeneficiaryId = beneficiary.Id;
        vm.FullName = beneficiary.FullName;
        vm.NationalId = beneficiary.NationalId;
        vm.AddressLine = beneficiary.AddressLine;
        vm.FamilyMembersCount = beneficiary.FamilyMembersCount;
        vm.MaritalStatus = beneficiary.MaritalStatus?.NameAr;
        vm.Age = beneficiary.BirthDate.HasValue
            ? Math.Max(0, DateTime.Today.Year - beneficiary.BirthDate.Value.Year)
            : null;
    }
    private async Task<BeneficiaryHeaderVm?> BuildBeneficiaryHeaderAsync(Guid? beneficiaryId)
    {
        if (!beneficiaryId.HasValue || beneficiaryId.Value == Guid.Empty)
            return null;

        var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId.Value);
        if (beneficiary == null)
            return null;

        return new BeneficiaryHeaderVm
        {
            Id = beneficiary.Id,
            Code = beneficiary.Code,
            FullName = beneficiary.FullName,
            NationalId = beneficiary.NationalId,
            PhoneNumber = beneficiary.PhoneNumber,
            StatusName = beneficiary.Status?.NameAr
        };
    }
    private static List<SelectListItem> BuildReviewDecisions() => new()
    {
        new() { Value = "Approve", Text = "موافقة" },
        new() { Value = "Reject", Text = "رفض" },
        new() { Value = "ReturnForRevision", Text = "إرجاع للتعديل" }
    };

    private static List<SelectListItem> BuildCommitteeDecisions() => new()
    {
        new() { Value = "Approve", Text = "موافقة" },
        new() { Value = "PartialApprove", Text = "موافقة جزئية" },
        new() { Value = "Reject", Text = "رفض" },
        new() { Value = "Suspend", Text = "تعليق" },
        new() { Value = "NeedMoreDocuments", Text = "استكمال مستندات" }
    };

    private async Task FillCommitteeAidTypesAsync(CommitteeBeneficiaryHumanitarianResearchVm vm)
    {
        var aidTypes = await _db.Set<AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.AidTypes = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = "-- اختر نوع المساعدة --" }
        };

        vm.AidTypes.AddRange(aidTypes.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = string.IsNullOrWhiteSpace(x.Category) ? x.NameAr : $"{x.NameAr} - {x.Category}",
            Selected = vm.ApprovedAidTypeId.HasValue && vm.ApprovedAidTypeId.Value == x.Id
        }));
    }

    private static bool IsCommitteeApprovalDecision(string? decision)
    {
        var value = (decision ?? string.Empty).Trim();

        return string.Equals(value, "Approve", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "Approved", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "ApproveAid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "PartialApprove", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "PartialApproved", StringComparison.OrdinalIgnoreCase)
            || value.Contains("موافقة", StringComparison.OrdinalIgnoreCase)
            || value.Contains("اعتماد", StringComparison.OrdinalIgnoreCase);
    }

    private async Task EnsureApprovedAidRequestAndDisbursementFromCommitteeDecisionAsync(
        BeneficiaryHumanitarianResearch research,
        AidTypeLookup aidType,
        CommitteeBeneficiaryHumanitarianResearchVm vm,
        string? userId)
    {
        const string sourceType = "HumanitarianResearchCommitteeDecision";

        var now = DateTime.UtcNow;
        var amount = vm.ApprovedAmount!.Value;
        var researchNumber = research.ResearchNumber ?? string.Empty;
        var note = $"تلقائي من قرار لجنة البحث الإنساني رقم {researchNumber}. {vm.Notes}".Trim();

        await using var tx = await _db.Database.BeginTransactionAsync();

        var committeeDecision = await _db.Set<BeneficiaryCommitteeDecision>()
            .FirstOrDefaultAsync(x => x.BeneficiaryId == research.BeneficiaryId
                && x.ApprovedAidTypeId == aidType.Id
                && x.DecisionType == "موافقة — بحث إنساني"
                && x.CommitteeNotes != null
                && x.CommitteeNotes.Contains(researchNumber));

        if (committeeDecision == null)
        {
            committeeDecision = new BeneficiaryCommitteeDecision
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = research.BeneficiaryId,
                DecisionDate = vm.CommitteeMeetingDate,
                DecisionType = "موافقة — بحث إنساني",
                ApprovedAidTypeId = aidType.Id,
                ApprovedAmount = amount,
                DurationInMonths = vm.DurationMonths,
                CommitteeNotes = note,
                ApprovedByUserId = userId,
                ApprovedStatus = true,
                CreatedAtUtc = now
            };

            _db.Set<BeneficiaryCommitteeDecision>().Add(committeeDecision);
        }
        else
        {
            committeeDecision.DecisionDate = vm.CommitteeMeetingDate;
            committeeDecision.ApprovedAmount = amount;
            committeeDecision.DurationInMonths = vm.DurationMonths;
            committeeDecision.CommitteeNotes = note;
            committeeDecision.ApprovedByUserId = userId;
            committeeDecision.ApprovedStatus = true;
        }

        var aidRequest = await _db.Set<BeneficiaryAidRequest>()
            .FirstOrDefaultAsync(x => x.BeneficiaryId == research.BeneficiaryId
                && x.AidTypeId == aidType.Id
                && x.Reason != null
                && x.Reason.Contains(researchNumber));

        if (aidRequest == null)
        {
            aidRequest = new BeneficiaryAidRequest
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = research.BeneficiaryId,
                RequestDate = vm.CommitteeMeetingDate,
                AidTypeId = aidType.Id,
                RequestedAmount = amount,
                Reason = note,
                UrgencyLevel = string.IsNullOrWhiteSpace(research.PriorityLevel) ? "Normal" : research.PriorityLevel.Trim(),
                Status = "Approved",
                CreatedByUserId = userId,
                CreatedAtUtc = now
            };

            _db.Set<BeneficiaryAidRequest>().Add(aidRequest);
        }
        else
        {
            aidRequest.RequestDate = vm.CommitteeMeetingDate;
            aidRequest.RequestedAmount = amount;
            aidRequest.Reason = note;
            aidRequest.UrgencyLevel = string.IsNullOrWhiteSpace(research.PriorityLevel) ? aidRequest.UrgencyLevel : research.PriorityLevel.Trim();
            aidRequest.Status = "Approved";
        }

        var disbursement = await _db.Set<BeneficiaryAidDisbursement>()
            .FirstOrDefaultAsync(x => x.SourceType == sourceType && x.SourceId == research.Id);

        if (disbursement == null)
        {
            disbursement = new BeneficiaryAidDisbursement
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = research.BeneficiaryId,
                AidRequestId = aidRequest.Id,
                AidTypeId = aidType.Id,
                DisbursementDate = vm.CommitteeMeetingDate,
                Amount = amount,
                ApprovalStatus = "Approved",
                ApprovedAtUtc = now,
                ApprovedByUserId = userId,
                ExecutionStatus = "Available",
                ExecutedAmount = 0m,
                SourceType = sourceType,
                SourceId = research.Id,
                Notes = note,
                CreatedByUserId = userId,
                CreatedAtUtc = now
            };

            _db.Set<BeneficiaryAidDisbursement>().Add(disbursement);
        }
        else
        {
            disbursement.BeneficiaryId = research.BeneficiaryId;
            disbursement.AidRequestId = aidRequest.Id;
            disbursement.AidTypeId = aidType.Id;
            disbursement.DisbursementDate = vm.CommitteeMeetingDate;
            disbursement.Amount = amount;
            disbursement.ApprovalStatus = "Approved";
            disbursement.ApprovedAtUtc ??= now;
            disbursement.ApprovedByUserId ??= userId;
            disbursement.ExecutionStatus = string.IsNullOrWhiteSpace(disbursement.ExecutionStatus) ? "Available" : disbursement.ExecutionStatus;

            if (!string.Equals(disbursement.ExecutionStatus, "FullyDisbursed", StringComparison.OrdinalIgnoreCase))
                disbursement.ExecutedAmount = 0m;

            disbursement.Notes = note;
        }

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private async Task<string> GenerateHumanCodeAsync()
    {
        string code;
        do
        {
            code = $"HUM-{DateTime.Now:yyyyMMddHHmmssfff}";
            await Task.Delay(5);
        }
        while (await _repository.ResearchNumberExistsAsync(code));

        return code;
    }

    private async Task<string> GenerateResearcherCodeAsync()
    {
        string code;
        do
        {
            code = $"RSR-{DateTime.Now:yyyyMMddHHmmssfff}";
            await Task.Delay(5);
        }
        while (await _db.Set<BeneficiaryHumanitarianResearch>()
            .AsNoTracking()
            .AnyAsync(x => x.ResearcherCode == code));

        return code;
    }

    private async Task<string> GenerateCommitteeCodeAsync()
    {
        string code;
        do
        {
            code = $"COM-{DateTime.Now:yyyyMMddHHmmssfff}";
            await Task.Delay(5);
        }
        while (await _db.Set<BeneficiaryHumanitarianResearch>()
            .AsNoTracking()
            .AnyAsync(x => x.CommitteeCode == code));

        return code;
    }

    private async Task<string> GetCurrentUserDisplayNameAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        var displayName = user?.DisplayName;

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = user?.UserName;

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return string.IsNullOrWhiteSpace(displayName) ? "system" : displayName.Trim();
    }
}
