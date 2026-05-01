using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.ProjectProposals;
using InfrastructureManagmentServices.Charity.ProjectProposals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skote.Models.Charity.ProjectProposals;
using System.Security.Claims;
using InfrastructureManagmentServices.Charity.Workflow;

namespace Skote.Controllers
{
    public class ProjectProposalsController : Controller
    {
        private readonly IProjectProposalRepository _repository;
        private readonly IProjectProposalConversionService _conversionService;
        private readonly IWorkflowService _workflow;

        public ProjectProposalsController(IProjectProposalRepository repository, IProjectProposalConversionService conversionService, IWorkflowService workflow)
        {
            _repository = repository;
            _conversionService = conversionService;
            _workflow = workflow;

        }

        public async Task<IActionResult> Index()
        {
            var items = await _repository.GetAllAsync();
            return View(items);
        }

        public IActionResult Create()
        {
            var vm = BuildDefaultVm();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectProposalVm vm)
        {
            if (vm.IsDraftSave)
            {
                // مسودة — أنشئ قيم افتراضية لو الحقول فاضية
                if (string.IsNullOrWhiteSpace(vm.ProposalNumber))
                    vm.ProposalNumber = $"DRAFT-{DateTime.Now:yyyyMMddHHmm}";
                if (string.IsNullOrWhiteSpace(vm.Title))
                    vm.Title = "(مسودة)";
                vm.Status = "Draft";
            }
            else
            {
                // تحقق يدوي من الحقول الأساسية فقط
                if (string.IsNullOrWhiteSpace(vm.ProposalNumber))
                    ModelState.AddModelError(nameof(vm.ProposalNumber), "رقم المقترح مطلوب.");
                if (string.IsNullOrWhiteSpace(vm.Title))
                    ModelState.AddModelError(nameof(vm.Title), "عنوان المقترح مطلوب.");
                if (vm.DurationMonths <= 0)
                    ModelState.AddModelError(nameof(vm.DurationMonths), "مدة التنفيذ يجب أن تكون أكبر من صفر.");
                if (vm.RequestedBudget < 0)
                    ModelState.AddModelError(nameof(vm.RequestedBudget), "الميزانية لا يمكن أن تكون سالبة.");

                // تحقق من المجموعات
                for (int idx = 0; idx < vm.Objectives.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Objectives[idx].Title))
                        ModelState.AddModelError($"Objectives[{idx}].Title", $"الهدف رقم {idx + 1}: العنوان مطلوب.");

                for (int idx = 0; idx < vm.Activities.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Activities[idx].Title))
                        ModelState.AddModelError($"Activities[{idx}].Title", $"النشاط رقم {idx + 1}: العنوان مطلوب.");

                for (int idx = 0; idx < vm.WorkPlanItems.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.WorkPlanItems[idx].ActivityTitle))
                        ModelState.AddModelError($"WorkPlanItems[{idx}].ActivityTitle", $"سطر خطة العمل رقم {idx + 1}: النشاط مطلوب.");

                for (int idx = 0; idx < vm.Phases.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Phases[idx].Name))
                        ModelState.AddModelError($"Phases[{idx}].Name", $"المرحلة رقم {idx + 1}: الاسم مطلوب.");

                if (!ModelState.IsValid)
                    return View(EnsureRows(vm));
            }

            if (await _repository.ProposalNumberExistsAsync(vm.ProposalNumber))
            {
                ModelState.AddModelError(nameof(vm.ProposalNumber), "رقم المقترح مستخدم من قبل.");
                return View(EnsureRows(vm));
            }

            var entity = new ProjectProposal();
            await MapVmToEntityAsync(vm, entity);
            entity.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _repository.AddAsync(entity);
            TempData["Success"] = vm.IsDraftSave ? "تم حفظ المسودة بنجاح." : "تم حفظ مقترح المشروع بنجاح.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();
            return View(MapEntityToVm(entity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectProposalVm vm)
        {
            if (!vm.Id.HasValue) return BadRequest();
            var entity = await _repository.GetByIdWithDetailsAsync(vm.Id.Value);
            vm.ProposalNumber = entity.ProposalNumber;
            if (entity == null) return NotFound();

            if (vm.IsDraftSave)
            {
                // مسودة — احتفظ بالقيم الموجودة لو الحقول فاضية
                if (string.IsNullOrWhiteSpace(vm.ProposalNumber))
                    vm.ProposalNumber = entity.ProposalNumber;
                if (string.IsNullOrWhiteSpace(vm.Title))
                    vm.Title = entity.Title;
                vm.Status = "Draft";
            }
            else
            {
                // تحقق يدوي من الحقول الأساسية فقط
                if (string.IsNullOrWhiteSpace(vm.ProposalNumber))
                    ModelState.AddModelError(nameof(vm.ProposalNumber), "رقم المقترح مطلوب.");
                if (string.IsNullOrWhiteSpace(vm.Title))
                    ModelState.AddModelError(nameof(vm.Title), "عنوان المقترح مطلوب.");
                if (vm.DurationMonths <= 0)
                    ModelState.AddModelError(nameof(vm.DurationMonths), "مدة التنفيذ يجب أن تكون أكبر من صفر.");
                if (vm.RequestedBudget < 0)
                    ModelState.AddModelError(nameof(vm.RequestedBudget), "الميزانية لا يمكن أن تكون سالبة.");

                for (int idx = 0; idx < vm.Objectives.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Objectives[idx].Title))
                        ModelState.AddModelError($"Objectives[{idx}].Title", $"الهدف رقم {idx + 1}: العنوان مطلوب.");

                for (int idx = 0; idx < vm.Activities.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Activities[idx].Title))
                        ModelState.AddModelError($"Activities[{idx}].Title", $"النشاط رقم {idx + 1}: العنوان مطلوب.");

                for (int idx = 0; idx < vm.WorkPlanItems.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.WorkPlanItems[idx].ActivityTitle))
                        ModelState.AddModelError($"WorkPlanItems[{idx}].ActivityTitle", $"سطر خطة العمل رقم {idx + 1}: النشاط مطلوب.");

                for (int idx = 0; idx < vm.Phases.Count; idx++)
                    if (string.IsNullOrWhiteSpace(vm.Phases[idx].Name))
                        ModelState.AddModelError($"Phases[{idx}].Name", $"المرحلة رقم {idx + 1}: الاسم مطلوب.");

                if (!ModelState.IsValid)
                    return View(EnsureRows(vm));
            }

            if (await _repository.ProposalNumberExistsAsync(vm.ProposalNumber, vm.Id))
            {
                ModelState.AddModelError(nameof(vm.ProposalNumber), "رقم المقترح مستخدم من قبل.");
                return View(EnsureRows(vm));
            }

            await MapVmToEntityAsync(vm, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            TempData["Success"] = vm.IsDraftSave ? "تم حفظ المسودة بنجاح." : "تم تحديث مقترح المشروع بنجاح.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();

            // تمرير خطوات الـ workflow للـ View
            var wfSteps = await _workflow.GetStepsAsync("ProjectProposal", id);
            ViewBag.WorkflowSteps   = wfSteps;
            ViewBag.WorkflowCurrent = wfSteps.FirstOrDefault(x => x.Status == "Pending");

            return View(entity);
        }

        // ── إرسال للمراجعة ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForReview(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (entity.Status is "UnderReview" or "Approved" or "ConvertedToProject")
            {
                TempData["Warning"] = "لا يمكن إرسال هذا المقترح للمراجعة في حالته الحالية.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // تغيير الحالة
            entity.Status      = "UnderReview";
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _repository.UpdateStatusAsync(id, "UnderReview");

            // بدء مسار الموافقة
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            await _workflow.InitiateAsync(
                entityType:        "ProjectProposal",
                entityId:          id,
                entityTitle:       $"مقترح: {entity.Title} ({entity.ProposalNumber})",
                submittedByUserId: userId
            );

            TempData["Success"] = "تم إرسال المقترح لمسار الموافقة بنجاح.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToProject(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();

            if (entity.CharityProjectId.HasValue)
            {
                TempData["Warning"] = "تم تحويل هذا المقترح إلى مشروع بالفعل.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // التحقق من اكتمال مسار الموافقة
            var pendingStep = await _workflow.GetCurrentStepAsync("ProjectProposal", id);
            if (pendingStep != null)
            {
                TempData["Warning"] = $"لا يمكن تحويل المقترح قبل اكتمال مسار الموافقة. الخطوة الحالية: «{pendingStep.StepName}»";
                return RedirectToAction(nameof(Details), new { id });
            }

            var wfSteps = await _workflow.GetStepsAsync("ProjectProposal", id);
            if (!wfSteps.Any() || !wfSteps.All(x => x.Status is "Approved" or "Skipped"))
            {
                TempData["Warning"] = "يجب إتمام مسار الموافقة أولاً قبل تحويل المقترح إلى مشروع.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var project = await _conversionService.ConvertToProjectAsync(entity, User.FindFirstValue(ClaimTypes.NameIdentifier));
            TempData["Success"] = $"تم تحويل المقترح إلى مشروع بنجاح. رقم المشروع: {project.Code}";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
        {
            // نجيب المرفق بشكل مباشر بدون تحميل باقي البيانات
            var attachment = await _repository.GetAttachmentContentAsync(attachmentId);
            if (attachment == null || attachment.ProjectProposalId != id) return NotFound();
            return File(attachment.FileContent, attachment.ContentType, attachment.OriginalFileName);
        }

        private ProjectProposalVm BuildDefaultVm()
        {
            var vm = new ProjectProposalVm();
            return EnsureRows(vm);
        }

        private ProjectProposalVm EnsureRows(ProjectProposalVm vm)
        {
            if (!vm.PastExperiences.Any()) vm.PastExperiences.Add(new ProjectProposalPastExperienceVm());
            if (!vm.TargetGroups.Any()) vm.TargetGroups.Add(new ProjectProposalTargetGroupVm());
            if (!vm.Objectives.Any()) vm.Objectives.Add(new ProjectProposalObjectiveVm { ObjectiveType = "Specific" });
            if (!vm.Activities.Any()) vm.Activities.Add(new ProjectProposalActivityVm());
            if (!vm.WorkPlanItems.Any()) vm.WorkPlanItems.Add(new ProjectProposalWorkPlanVm());
            if (!vm.MonitoringIndicators.Any()) vm.MonitoringIndicators.Add(new ProjectProposalMonitoringIndicatorVm());
            if (!vm.TeamMembers.Any()) vm.TeamMembers.Add(new ProjectProposalTeamMemberVm());
            return vm;
        }

        private ProjectProposalVm MapEntityToVm(ProjectProposal entity)
        {
            var orderedPhases = entity.Phases.OrderBy(p => p.SortOrder).ToList();

            return EnsureRows(new ProjectProposalVm
            {
                Id                        = entity.Id,
                ProposalNumber            = entity.ProposalNumber,
                Title                     = entity.Title,
                DonorName                 = entity.DonorName,
                OrganizationName          = entity.OrganizationName,
                ProjectLocation           = entity.ProjectLocation,
                SubmissionDate            = entity.SubmissionDate,
                DurationMonths            = entity.DurationMonths,
                RequestedBudget           = entity.RequestedBudget,
                Currency                  = entity.Currency,
                Status                    = entity.Status,
                RegistrationNumber        = entity.RegistrationNumber,
                RegistrationYear          = entity.RegistrationYear,
                Vision                    = entity.Vision,
                Mission                   = entity.Mission,
                ExpertiseSummary          = entity.ExpertiseSummary,
                EmployeesCount            = entity.EmployeesCount,
                VolunteersCount           = entity.VolunteersCount,
                YearsOfExperience         = entity.YearsOfExperience,
                ContactPerson             = entity.ContactPerson,
                ContactPhone              = entity.ContactPhone,
                ContactEmail              = entity.ContactEmail,
                Address                   = entity.Address,
                ProblemBackground         = entity.ProblemBackground,
                ProblemAnalysis           = entity.ProblemAnalysis,
                NationalAlignment         = entity.NationalAlignment,
                ProposedApproach          = entity.ProposedApproach,
                ProposedSolution          = entity.ProposedSolution,
                RisksAndExternalFactors   = entity.RisksAndExternalFactors,
                ExecutiveSummary          = entity.ExecutiveSummary,
                GeneralGoal               = entity.GeneralGoal,
                ExpectedResults           = entity.ExpectedResults,
                PreparatoryRequirements   = entity.PreparatoryRequirements,
                ImplementationTeamSummary = entity.ImplementationTeamSummary,
                SustainabilityPlan        = entity.SustainabilityPlan,
                Notes                     = entity.Notes,

                PastExperiences = entity.PastExperiences
                    .Select(x => new ProjectProposalPastExperienceVm
                    {
                        ProjectName = x.ProjectName, Donor = x.Donor, Location = x.Location,
                        DurationText = x.DurationText, Budget = x.Budget,
                        TargetGroup = x.TargetGroup, ResultAchieved = x.ResultAchieved
                    }).ToList(),

                TargetGroups = entity.TargetGroups
                    .Select(x => new ProjectProposalTargetGroupVm
                    {
                        CategoryName = x.CategoryName, TargetCount = x.TargetCount,
                        AgeRange = x.AgeRange, GenderNotes = x.GenderNotes,
                        SelectionCriteria = x.SelectionCriteria, BenefitDescription = x.BenefitDescription
                    }).ToList(),

                Objectives = entity.Objectives
                    .Select(x => new ProjectProposalObjectiveVm
                    {
                        ObjectiveType = x.ObjectiveType, Title = x.Title, Description = x.Description
                    }).ToList(),

                Activities = entity.Activities.OrderBy(x => x.SortOrder)
                    .Select(x => new ProjectProposalActivityVm
                    {
                        Title             = x.Title,
                        Description       = x.Description,
                        ResponsibleRole   = x.ResponsibleRole,
                        NeededResources   = x.NeededResources,
                        PlannedStartMonth = x.PlannedStartMonth,
                        PlannedEndMonth   = x.PlannedEndMonth,
                        Responsible       = x.Responsible,
                        TargetGroup       = x.TargetGroup,
                        PlannedCount      = x.PlannedCount,
                        Priority          = x.Priority
                    }).ToList(),

                WorkPlanItems = entity.WorkPlanItems
                    .Select(x => new ProjectProposalWorkPlanVm
                    {
                        GoalTitle = x.GoalTitle, ActivityTitle = x.ActivityTitle,
                        PhaseName = x.PhaseName,
                        ContributionPercent = x.ContributionPercent,
                        PlannedQuantity = x.PlannedQuantity,
                        DurationDays = x.DurationDays,
                        Responsible = x.Responsible, Resources = x.Resources,
                        StartMonth = x.StartMonth, EndMonth = x.EndMonth
                    }).ToList(),

                MonitoringIndicators = entity.MonitoringIndicators
                    .Select(x => new ProjectProposalMonitoringIndicatorVm
                    {
                        ActivityTitle      = x.ActivityTitle,
                        Indicator          = x.Indicator,
                        TargetValue        = x.TargetValue,
                        AchievedValue      = null,                // لا يوجد في الـ entity حالياً
                        VerificationMethod = x.VerificationMethod,
                        VerificationMeans  = x.VerificationMethod, // نفس الحقل — مزدوج في الـ form
                        ReportingFrequency = x.ReportingFrequency
                    }).ToList(),

                TeamMembers = entity.TeamMembers
                    .Select(x => new ProjectProposalTeamMemberVm
                    {
                        RoleName = x.RoleName, Responsibility = x.Responsibility, IsInternal = x.IsInternal
                    }).ToList(),

                // المراحل — ActivityTitles تُستخدم لتعبئة الـ select في الـ form
                Phases = orderedPhases
                    .Select(p => new ProposalPhaseVm
                    {
                        Name           = p.Name,
                        Description    = p.Description,
                        StartMonth     = p.StartMonth,
                        EndMonth       = p.EndMonth,
                        ActivityTitles = string.Join(",", p.Activities.OrderBy(a => a.SortOrder).Select(a => a.ActivityTitle))
                    }).ToList(),

                // PhaseActivities — لتعبئة قيم المساهمة والكميات عند إعادة فتح الـ form
                PhaseActivities = orderedPhases
                    .Select(p => p.Activities.OrderBy(a => a.SortOrder)
                        .Select(a => new ProposalPhaseActivityRowVm
                        {
                            ActivityTitle   = a.ActivityTitle,
                            ContributionPct = a.ContributionPercent,
                            PlannedQuantity = a.PlannedQuantity,
                            DurationDays    = a.DurationDays
                        }).ToList()
                    ).ToList(),

                ExistingAttachments = entity.Attachments
                    .Select(x => new ProjectProposalAttachmentRowVm
                    {
                        Id = x.Id, OriginalFileName = x.OriginalFileName,
                        AttachmentType = x.AttachmentType, FileSizeBytes = x.FileSizeBytes
                    }).ToList()
            });
        }

        private async Task MapVmToEntityAsync(ProjectProposalVm vm, ProjectProposal entity)
        {
            entity.ProposalNumber            = vm.ProposalNumber;
            entity.Title                     = vm.Title;
            entity.DonorName                 = vm.DonorName;
            entity.OrganizationName          = vm.OrganizationName;
            entity.ProjectLocation           = vm.ProjectLocation;
            entity.SubmissionDate            = vm.SubmissionDate;
            entity.DurationMonths            = vm.DurationMonths;
            entity.RequestedBudget           = vm.RequestedBudget;
            entity.Currency                  = vm.Currency;
            entity.Status                    = vm.Status;
            entity.RegistrationNumber        = vm.RegistrationNumber;
            entity.RegistrationYear          = vm.RegistrationYear;
            entity.Vision                    = vm.Vision;
            entity.Mission                   = vm.Mission;
            entity.ExpertiseSummary          = vm.ExpertiseSummary;
            entity.EmployeesCount            = vm.EmployeesCount;
            entity.VolunteersCount           = vm.VolunteersCount;
            entity.YearsOfExperience         = vm.YearsOfExperience;
            entity.ContactPerson             = vm.ContactPerson;
            entity.ContactPhone              = vm.ContactPhone;
            entity.ContactEmail              = vm.ContactEmail;
            entity.Address                   = vm.Address;
            entity.ProblemBackground         = vm.ProblemBackground;
            entity.ProblemAnalysis           = vm.ProblemAnalysis;
            entity.NationalAlignment         = vm.NationalAlignment;
            entity.ProposedApproach          = vm.ProposedApproach;
            entity.ProposedSolution          = vm.ProposedSolution;
            entity.RisksAndExternalFactors   = vm.RisksAndExternalFactors;
            entity.ExecutiveSummary          = vm.ExecutiveSummary;
            entity.GeneralGoal               = vm.GeneralGoal;
            entity.ExpectedResults           = vm.ExpectedResults;
            entity.PreparatoryRequirements   = vm.PreparatoryRequirements;
            entity.ImplementationTeamSummary = vm.ImplementationTeamSummary;
            entity.SustainabilityPlan        = vm.SustainabilityPlan;
            entity.Notes                     = vm.Notes;

            entity.PastExperiences = vm.PastExperiences
                .Where(x => !string.IsNullOrWhiteSpace(x.ProjectName))
                .Select(x => new ProjectProposalPastExperience
                {
                    Id = Guid.NewGuid(), ProjectName = x.ProjectName, Donor = x.Donor,
                    Location = x.Location, DurationText = x.DurationText,
                    Budget = x.Budget, TargetGroup = x.TargetGroup, ResultAchieved = x.ResultAchieved
                }).ToList();

            entity.TargetGroups = vm.TargetGroups
                .Where(x => !string.IsNullOrWhiteSpace(x.CategoryName))
                .Select(x => new ProjectProposalTargetGroup
                {
                    Id = Guid.NewGuid(), CategoryName = x.CategoryName, TargetCount = x.TargetCount,
                    AgeRange = x.AgeRange, GenderNotes = x.GenderNotes,
                    SelectionCriteria = x.SelectionCriteria, BenefitDescription = x.BenefitDescription
                }).ToList();

            entity.Objectives = vm.Objectives
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .Select(x => new ProjectProposalObjective
                {
                    Id = Guid.NewGuid(), ObjectiveType = x.ObjectiveType,
                    Title = x.Title, Description = x.Description
                }).ToList();

            entity.Activities = vm.Activities
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .Select((x, idx) => new ProjectProposalActivity
                {
                    Id               = Guid.NewGuid(),
                    SortOrder        = idx,
                    Title            = x.Title,
                    Description      = x.Description,
                    ResponsibleRole  = x.ResponsibleRole,
                    NeededResources  = x.NeededResources,
                    PlannedStartMonth = x.PlannedStartMonth,
                    PlannedEndMonth  = x.PlannedEndMonth,
                    Responsible      = x.Responsible,
                    TargetGroup      = x.TargetGroup,
                    PlannedCount     = x.PlannedCount,
                    Priority         = x.Priority ?? "Medium"
                }).ToList();

            entity.WorkPlanItems = vm.WorkPlanItems
                .Where(x => !string.IsNullOrWhiteSpace(x.ActivityTitle))
                .Select(x => new ProjectProposalWorkPlan
                {
                    Id = Guid.NewGuid(), GoalTitle = x.GoalTitle, ActivityTitle = x.ActivityTitle,
                    PhaseName = x.PhaseName,
                    ContributionPercent = x.ContributionPercent,
                    PlannedQuantity = x.PlannedQuantity,
                    DurationDays = x.DurationDays,
                    Responsible = x.Responsible, Resources = x.Resources,
                    StartMonth = x.StartMonth, EndMonth = x.EndMonth
                }).ToList();

            entity.MonitoringIndicators = vm.MonitoringIndicators
                .Where(x => !string.IsNullOrWhiteSpace(x.Indicator))
                .Select(x => new ProjectProposalMonitoringIndicator
                {
                    Id                 = Guid.NewGuid(),
                    ActivityTitle      = x.ActivityTitle,
                    Indicator          = x.Indicator,
                    TargetValue        = x.TargetValue,
                    VerificationMethod = x.VerificationMeans ?? x.VerificationMethod,
                    ReportingFrequency = x.ReportingFrequency
                }).ToList();

            entity.TeamMembers = vm.TeamMembers
                .Where(x => !string.IsNullOrWhiteSpace(x.RoleName))
                .Select(x => new ProjectProposalTeamMember
                {
                    Id = Guid.NewGuid(), RoleName = x.RoleName,
                    Responsibility = x.Responsibility, IsInternal = x.IsInternal
                }).ToList();

            // المراحل — يجمع بيانات vm.Phases مع vm.PhaseActivities
            entity.Phases = vm.Phases
                .Select((ph, pi) => new ProjectProposalPhase
                {
                    Id                 = Guid.NewGuid(),
                    SortOrder          = pi,
                    Name               = ph.Name,
                    Description        = ph.Description,
                    StartMonth         = ph.StartMonth,
                    EndMonth           = ph.EndMonth,
                    Activities         = (pi < vm.PhaseActivities.Count ? vm.PhaseActivities[pi] : new())
                        .Where(a => !string.IsNullOrWhiteSpace(a.ActivityTitle))
                        .Select((a, aj) => new ProjectProposalPhaseActivity
                        {
                            Id                 = Guid.NewGuid(),
                            SortOrder          = aj,
                            ActivityTitle      = a.ActivityTitle,
                            ContributionPercent = a.ContributionPct,
                            PlannedQuantity    = a.PlannedQuantity,
                            DurationDays       = a.DurationDays
                        }).ToList()
                })
                .Where(ph => !string.IsNullOrWhiteSpace(ph.Name))
                .ToList();

            if (vm.NewAttachments != null && vm.NewAttachments.Any())
            {
                foreach (var file in vm.NewAttachments.Where(f => f != null && f.Length > 0))
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    entity.Attachments.Add(new ProjectProposalAttachment
                    {
                        Id = Guid.NewGuid(),
                        OriginalFileName = file.FileName,
                        ContentType = file.ContentType ?? "application/octet-stream",
                        FileExtension = Path.GetExtension(file.FileName),
                        FileSizeBytes = file.Length,
                        FileContent = ms.ToArray(),
                        AttachmentType = "General",
                        CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    });
                }
            }
        }
    }
}
