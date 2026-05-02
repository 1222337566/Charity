using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Beneficiaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.ProjectsView)]
public class ProjectBeneficiariesController : Controller
{
    private readonly IProjectBeneficiaryRepository _projectBeneficiaryRepository;
    private readonly ICharityProjectRepository     _projectRepository;
    private readonly IBeneficiaryRepository        _beneficiaryRepository;
    private readonly AppDbContext                  _db;

    public ProjectBeneficiariesController(
        IProjectBeneficiaryRepository projectBeneficiaryRepository,
        ICharityProjectRepository projectRepository,
        IBeneficiaryRepository beneficiaryRepository,
        AppDbContext db)
    {
        _projectBeneficiaryRepository = projectBeneficiaryRepository;
        _projectRepository            = projectRepository;
        _beneficiaryRepository        = beneficiaryRepository;
        _db                           = db;
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId)) return NotFound();
        var items = await _projectBeneficiaryRepository.GetByProjectIdAsync(projectId);
        return View(items.Select(x => new ProjectBeneficiaryListItemVm
        {
            Id              = x.Id,
            BeneficiaryId   = x.BeneficiaryId,
            BeneficiaryCode = x.Beneficiary?.Code     ?? string.Empty,
            BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
            EnrollmentDate  = x.EnrollmentDate,
            ExitDate        = x.ExitDate,
            BenefitType     = x.BenefitType,
            Notes           = x.Notes
        }).ToList());
    }

    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpGet]
    public async Task<IActionResult> Create(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId)) return NotFound();
        var vm = new CreateProjectBeneficiaryVm
        {
            ProjectId      = projectId,
            EnrollmentDate = DateTime.Today
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectBeneficiaryVm vm)
    {
        if (!await PopulateProjectAsync(vm.ProjectId)) return NotFound();
        await FillLookupsAsync(vm);
        await ValidateCommitteeDecisionAsync(vm.BeneficiaryId);

        if (!ModelState.IsValid) return View(vm);

        if (await _projectBeneficiaryRepository.ExistsAsync(vm.ProjectId, vm.BeneficiaryId))
        {
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مضاف بالفعل إلى هذا المشروع");
            return View(vm);
        }

        var pb = new ProjectBeneficiary
        {
            Id              = Guid.NewGuid(),
            ProjectId       = vm.ProjectId,
            BeneficiaryId   = vm.BeneficiaryId,
            EnrollmentDate  = vm.EnrollmentDate,
            ExitDate        = vm.ExitDate,
            BenefitType     = vm.BenefitType?.Trim(),
            TargetGroupName = vm.TargetGroupName?.Trim(),
            Notes           = vm.Notes?.Trim()
        };
        await _projectBeneficiaryRepository.AddAsync(pb);

        // ── ربط بالنشاط تلقائياً لو تم تحديده ──
        if (vm.ActivityId.HasValue && vm.PhaseId.HasValue)
        {
            var actBen = new ProjectActivityBeneficiary
            {
                Id                 = Guid.NewGuid(),
                ProjectId          = vm.ProjectId,
                PhaseId            = vm.PhaseId.Value,
                ActivityId         = vm.ActivityId.Value,
                BeneficiaryId      = pb.Id,
                TargetGroupName    = vm.TargetGroupName?.Trim(),
                ParticipationType  = vm.ParticipationType,
                VerificationStatus = "Unverified",
                CreatedAtUtc       = DateTime.UtcNow
            };
            _db.Set<ProjectActivityBeneficiary>().Add(actBen);
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "تم ربط المستفيد بالمشروع";
        return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
    }

    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _projectBeneficiaryRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();

        var vm = new EditProjectBeneficiaryVm
        {
            Id              = entity.Id,
            ProjectId       = entity.ProjectId,
            BeneficiaryId   = entity.BeneficiaryId,
            EnrollmentDate  = entity.EnrollmentDate,
            ExitDate        = entity.ExitDate,
            BenefitType     = entity.BenefitType,
            Notes           = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProjectBeneficiaryVm vm)
    {
        var entity = await _projectBeneficiaryRepository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();

        await FillLookupsAsync(vm);
        await ValidateCommitteeDecisionAsync(vm.BeneficiaryId);
        if (!ModelState.IsValid) return View(vm);

        if (entity.BeneficiaryId != vm.BeneficiaryId &&
            await _projectBeneficiaryRepository.ExistsAsync(entity.ProjectId, vm.BeneficiaryId))
        {
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مضاف بالفعل إلى هذا المشروع");
            return View(vm);
        }

        entity.BeneficiaryId  = vm.BeneficiaryId;
        entity.EnrollmentDate = vm.EnrollmentDate;
        entity.ExitDate       = vm.ExitDate;
        entity.BenefitType    = vm.BenefitType?.Trim();
        entity.Notes          = vm.Notes?.Trim();
        await _projectBeneficiaryRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل بيانات ربط المستفيد";
        return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
    }

    // ── Verify — إثبات مشاركة مستفيد ─────────────────────────────────────────
    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpGet]
    public async Task<IActionResult> Verify(Guid activityBeneficiaryId)
    {
        var ab = await _db.Set<ProjectActivityBeneficiary>()
            .Include(x => x.Activity)
            .Include(x => x.Phase)
            .Include(x => x.Beneficiary).ThenInclude(b => b!.Beneficiary)
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == activityBeneficiaryId);
        if (ab == null) return NotFound();

        if (!await PopulateProjectAsync(ab.ProjectId)) return NotFound();

        ViewBag.ActivityBeneficiary = ab;
        return View(new VerifyActivityBeneficiaryVm
        {
            ActivityBeneficiaryId = ab.Id,
            VerificationStatus    = ab.VerificationStatus,
            VerificationNotes     = ab.VerificationNotes,
            VerificationDate      = ab.VerificationDate ?? DateTime.Today
        });
    }

    [Authorize(Policy = CharityPolicies.ProjectsManage)][HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(VerifyActivityBeneficiaryVm vm)
    {
        var ab = await _db.Set<ProjectActivityBeneficiary>()
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityBeneficiaryId);
        if (ab == null) return NotFound();

        ab.VerificationStatus  = vm.VerificationStatus;
        ab.VerificationNotes   = vm.VerificationNotes?.Trim();
        ab.VerificationDate    = vm.VerificationDate;
        ab.VerifiedByUserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        ab.UpdatedAtUtc        = DateTime.UtcNow;
        _db.Set<ProjectActivityBeneficiary>().Update(ab);

        // رفع المرفقات
        if (vm.NewAttachments?.Count > 0)
        {
            foreach (var file in vm.NewAttachments)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                _db.Set<ProjectActivityBeneficiaryAttachment>().Add(new()
                {
                    Id                      = Guid.NewGuid(),
                    ActivityBeneficiaryId   = ab.Id,
                    AttachmentType          = vm.AttachmentType ?? "Other",
                    OriginalFileName        = file.FileName,
                    ContentType             = file.ContentType ?? "application/octet-stream",
                    FileSizeBytes           = file.Length,
                    FileContent             = ms.ToArray(),
                    UploadedByUserId        = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    CreatedAtUtc            = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "تم حفظ حالة الإثبات";
        return RedirectToAction(nameof(Index), new { projectId = ab.ProjectId });
    }

    // ── API: get activities by phase (AJAX) ───────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetActivitiesByPhase(Guid phaseId)
    {
        var acts = await _db.Set<ActivityPhaseAssignment>()
            .Where(a => a.PhaseId == phaseId)
            .Include(a => a.Activity)
            .Select(a => new { value = a.Activity!.Id, text = a.Activity.Title })
            .ToListAsync();
        return Json(acts);
    }

    // ── أنشطة مستفيد: عرض كل الأنشطة المسكَّن فيها ───────────────────────
    [HttpGet]
    public async Task<IActionResult> BeneficiaryActivities(Guid projectBeneficiaryId)
    {
        var pb = await _db.Set<ProjectBeneficiary>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == projectBeneficiaryId);
        if (pb == null) return NotFound();

        // كل الأنشطة التي تم تسكين هذا المستفيد فيها
        var activities = await _db.Set<ProjectActivityBeneficiary>()
            .AsNoTracking()
            .Include(ab => ab.Activity)
            .Include(ab => ab.Phase)
            .Where(ab => ab.BeneficiaryId == projectBeneficiaryId)
            .OrderBy(ab => ab.Phase!.SortOrder)
            .ToListAsync();

        ViewBag.ProjectBeneficiary = pb;
        ViewBag.ProjectId = pb.ProjectId;

        // إعداد header
        var project = await _projectRepository.GetByIdAsync(pb.ProjectId);
        if (project != null)
            ViewBag.ProjectHeader = new ProjectHeaderVm
            {
                Id = project.Id, Code = project.Code, Name = project.Name,
                Status = project.Status, StartDate = project.StartDate,
                EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive
            };

        return View(activities);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private async Task ValidateCommitteeDecisionAsync(Guid beneficiaryId)
    {
        if (beneficiaryId == Guid.Empty) return;
        var validation = await BeneficiaryCommitteeDecisionGuard.ValidateAsync(
            _db, beneficiaryId, null, null, "ربط المستفيد بالمشروع");
        if (!validation.IsValid)
            ModelState.AddModelError(nameof(CreateProjectBeneficiaryVm.BeneficiaryId), validation.Message);
    }

    private async Task FillLookupsAsync(CreateProjectBeneficiaryVm vm)
    {
        // المستفيدون
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        vm.Beneficiaries = beneficiaries.OrderBy(x => x.Code)
            .Select(x => new SelectListItem(x.Code + " - " + x.FullName, x.Id.ToString())).ToList();

        // الفئات المستهدفة من المقترح المرتبط بالمشروع
        var targetGroups = await GetProjectTargetGroupsAsync(vm.ProjectId);
        vm.TargetGroups = targetGroups
            .Select(t => new SelectListItem(t, t)).ToList();

        // المراحل
        var phases = await _db.Set<ProjectPhase>()
            .AsNoTracking().Where(p => p.ProjectId == vm.ProjectId && p.IsActive)
            .OrderBy(p => p.SortOrder).ToListAsync();
        vm.Phases = phases
            .Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();
    }

    private async Task FillLookupsAsync(EditProjectBeneficiaryVm vm)
    {
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        vm.Beneficiaries = beneficiaries.OrderBy(x => x.Code)
            .Select(x => new SelectListItem(x.Code + " - " + x.FullName, x.Id.ToString())).ToList();
    }

    /// <summary>يجيب الفئات المستهدفة من المقترح المرتبط بالمشروع</summary>
    private async Task<List<string>> GetProjectTargetGroupsAsync(Guid projectId)
    {
        var proposal = await _db.Set<CharityProject>()
            .Where(p => p.Id == projectId)
            .Join(_db.Set<ProjectProposal>(),
                  proj => proj.Code,
                  prop => prop.ProposalNumber,
                  (proj, prop) => prop)
            .Include(p => p.TargetGroups)
            .FirstOrDefaultAsync();

        if (proposal == null)
        {
            // fallback: من TargetGroup المذكور في الأنشطة
            return await _db.Set<ProjectSubGoalActivity>()
                .Where(a => a.ProjectId == projectId && !string.IsNullOrEmpty(a.TargetGroup))
                .Select(a => a.TargetGroup!)
                .Distinct().ToListAsync();
        }

        return proposal.TargetGroups
            .Where(t => !string.IsNullOrWhiteSpace(t.CategoryName))
            .Select(t => t.CategoryName!)
            .Distinct().ToList();
    }

    private async Task<bool> PopulateProjectAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null) return false;
        ViewBag.ProjectHeader = new ProjectHeaderVm
        {
            Id        = project.Id,   Code   = project.Code,
            Name      = project.Name, Status = project.Status,
            StartDate = project.StartDate, EndDate = project.EndDate,
            Budget    = project.Budget,   IsActive = project.IsActive
        };
        return true;
    }
}
