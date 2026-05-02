using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class ProjectGoalsController : Controller
    {
        private readonly AppDbContext _db;
        public ProjectGoalsController(AppDbContext db) => _db = db;

        // ══════════════════════════════════════════════════
        //  الصفحة الرئيسية — شجرة الأهداف الكاملة
        // ══════════════════════════════════════════════════
        public async Task<IActionResult> Index(Guid projectId, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return NotFound();

            var goals = await _db.Set<ProjectGoal>()
                .AsNoTracking()
                .Include(g => g.SubGoals.Where(s => s.IsActive))
                    .ThenInclude(s => s.Activities.Where(a => a.IsActive))
                        .ThenInclude(a => a.PhaseAssignments)
                            .ThenInclude(pa => pa.Phase)
                .Where(g => g.ProjectId == projectId && g.IsActive)
                .OrderBy(g => g.SortOrder).ToListAsync(ct);

            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId && p.IsActive)
                .OrderBy(p => p.SortOrder).ToListAsync(ct);

            ViewBag.Project = project;
            ViewBag.Phases = phases;
            ViewBag.PhasesJson = System.Text.Json.JsonSerializer.Serialize(
                phases.Select(p => new { id = p.Id, name = p.Name }));
            return View(goals);
        }

        // ══════════════════════════════════════════════════
        // CRUD — أهداف رئيسية
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGoal(Guid projectId, string title,
            string? description, string? successIndicator, string? targetValue, int sortOrder)
        {
            _db.Set<ProjectGoal>().Add(new ProjectGoal
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = title.Trim(),
                Description = description?.Trim(),
                SuccessIndicator = successIndicator?.Trim(),
                TargetValue = targetValue?.Trim(),
                SortOrder = sortOrder > 0 ? sortOrder : 99,
                Status = "Active",
                IsActive = true
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة الهدف الرئيسي «{title}»";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectGoal>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ══════════════════════════════════════════════════
        // CRUD — أهداف فرعية
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubGoal(Guid projectId, Guid goalId,
            string title, string? description, string? successIndicator,
            string? targetValue, int sortOrder)
        {
            _db.Set<ProjectSubGoal>().Add(new ProjectSubGoal
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                GoalId = goalId,
                Title = title.Trim(),
                Description = description?.Trim(),
                SuccessIndicator = successIndicator?.Trim(),
                TargetValue = targetValue?.Trim(),
                SortOrder = sortOrder > 0 ? sortOrder : 99,
                Status = "Active",
                IsActive = true
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة الهدف الفرعي «{title}»";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubGoal(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectSubGoal>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ══════════════════════════════════════════════════
        // CRUD — الأنشطة
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(
            Guid projectId, Guid subGoalId, string title, string? description,
            string? targetGroup, decimal plannedQuantity, string? quantityUnit,
            int? plannedDurationDays, decimal? plannedHoursPerDay,
            decimal plannedCost, string priority,
            string? performanceIndicator, string? verificationMeans,
            DateTime? plannedStartDate, DateTime? plannedEndDate,
            string? responsiblePersonName, int sortOrder)
        {
            var act = new ProjectSubGoalActivity
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                SubGoalId = subGoalId,
                Title = title.Trim(),
                Description = description?.Trim(),
                TargetGroup = targetGroup?.Trim(),
                PlannedQuantity = (int)plannedQuantity,
                QuantityUnit = quantityUnit?.Trim(),
                PlannedDurationDays = (int)plannedDurationDays,
                PlannedHoursPerDay = (int)plannedHoursPerDay,
                PlannedCost = plannedCost,
                Priority = priority,
                PlannedStartDate = plannedStartDate,
                PlannedEndDate = plannedEndDate,
                PerformanceIndicator = performanceIndicator?.Trim(),
                VerificationMeans = verificationMeans?.Trim(),
                ResponsiblePersonName = responsiblePersonName?.Trim(),
                SortOrder = sortOrder > 0 ? sortOrder : 99,
                Status = "Planned",
                IsActive = true
            };
            _db.Set<ProjectSubGoalActivity>().Add(act);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة النشاط «{title}» — يمكنك ربطه بمراحل التنفيذ";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateActivity(
            Guid id, Guid projectId, decimal progress, string status,
            decimal actualQuantity, decimal actualCost, string? notes)
        {
            var act = await _db.Set<ProjectSubGoalActivity>().FindAsync(id);
            if (act == null) return NotFound();
            act.ProgressPercent = Math.Min(progress, 100);
            act.Status = status; act.ActualQuantity = (int)actualQuantity;
            act.ActualCost = actualCost; act.Notes = notes;
            act.UpdatedAtUtc = DateTime.UtcNow;
            if (status == "InProgress" && act.ActualStartDate == null)
                act.ActualStartDate = DateTime.Today;
            if (status == "Completed") act.ActualEndDate = DateTime.Today;
            await RecalcSubGoalProgressAsync(act.SubGoalId);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حفظ التقدم";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActivity(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectSubGoalActivity>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ══════════════════════════════════════════════════
        // ربط النشاط بمرحلة
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPhase(
            Guid activityId, Guid projectId, Guid phaseId,
            decimal plannedQuantity, decimal contributionPercent, int sortOrder,
            int? plannedDurationDays, decimal? plannedHoursPerDay,
            decimal plannedCost, DateTime? plannedStartDate, DateTime? plannedEndDate)
        {
            var exists = await _db.Set<ActivityPhaseAssignment>()
                .AnyAsync(x => x.ActivityId == activityId && x.PhaseId == phaseId);
            if (exists) { TempData["Error"] = "النشاط مرتبط بهذه المرحلة مسبقاً"; return RedirectToAction(nameof(Index), new { projectId }); }

            _db.Set<ActivityPhaseAssignment>().Add(new ActivityPhaseAssignment
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                PhaseId = phaseId,
                PlannedQuantity = plannedQuantity,
                ContributionPercent = contributionPercent > 0 ? contributionPercent : 100,
                SortOrder = sortOrder > 0 ? sortOrder : 1,
                PlannedDurationDays = plannedDurationDays,
                PlannedHoursPerDay = plannedHoursPerDay,
                PlannedCost = plannedCost,
                PlannedStartDate = plannedStartDate,
                PlannedEndDate = plannedEndDate,
                Status = "Planned"
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم ربط النشاط بالمرحلة";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhaseProgress(
            Guid assignmentId, Guid projectId, decimal progress,
            string status, decimal actualQuantity, decimal actualCost, int? actualDurationDays)
        {
            var pa = await _db.Set<ActivityPhaseAssignment>().FindAsync(assignmentId);
            if (pa != null)
            {
                pa.ProgressPercent = progress; pa.Status = status;
                pa.ActualQuantity = actualQuantity; pa.ActualCost = actualCost;
                pa.ActualDurationDays = actualDurationDays;
                if (status == "InProgress" && pa.ActualStartDate == null)
                    pa.ActualStartDate = DateTime.Today;
                if (status == "Completed") pa.ActualEndDate = DateTime.Today;
                await _db.SaveChangesAsync();

                // احسب تقدم النشاط الكلي من مجموع المراحل
                var allPa = await _db.Set<ActivityPhaseAssignment>()
                    .Where(x => x.ActivityId == pa.ActivityId).ToListAsync();
                var act = await _db.Set<ProjectSubGoalActivity>().FindAsync(pa.ActivityId);
                if (act != null && allPa.Any())
                {
                    act.ProgressPercent = allPa.Sum(p => p.ProgressPercent * p.ContributionPercent / 100);
                    act.ActualQuantity = (int)allPa.Sum(p => p.ActualQuantity);
                    act.ActualCost = allPa.Sum(p => p.ActualCost);
                    await RecalcSubGoalProgressAsync(act.SubGoalId);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhase(Guid assignmentId, Guid projectId)
        {
            var e = await _db.Set<ActivityPhaseAssignment>().FindAsync(assignmentId);
            if (e != null) { _db.Set<ActivityPhaseAssignment>().Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ══════════════════════════════════════════════════
        // إضافة مستفيد سريعة داخل المشروع
        // ══════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> QuickAddBeneficiary(Guid projectId)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return NotFound();
            ViewBag.Project = project;
            var vm = new QuickBeneficiaryVm { ProjectId = projectId };
            vm.TargetGroups = await GetProjectTargetGroupsAsync(projectId);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAddBeneficiary(QuickBeneficiaryVm vm, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>().FindAsync(vm.ProjectId);
            ViewBag.Project = project;
            vm.TargetGroups = await GetProjectTargetGroupsAsync(vm.ProjectId);

            if (!ModelState.IsValid) return View(vm);

            // ابحث عن مستفيد موجود بالرقم القومي أو الاسم
            Guid beneficiaryId;
            var existing = !string.IsNullOrWhiteSpace(vm.NationalId)
                ? await _db.Set<Beneficiary>().FirstOrDefaultAsync(x => x.NationalId == vm.NationalId, ct)
                : null;

            if (existing != null)
            {
                beneficiaryId = existing.Id;
            }
            else
            {
                // أنشئ مستفيد جديد
                var code = $"BN-{DateTime.UtcNow:yyMMddHHmmss}";
                var newBen = new Beneficiary
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    FullName = vm.FullName.Trim(),
                    NationalId = vm.NationalId?.Trim(),
                    PhoneNumber = vm.PhoneNumber?.Trim(),
                    FamilyMembersCount = vm.FamilyMembersCount,
                    MonthlyIncome = vm.MonthlyIncome,
                    RegistrationDate = DateTime.Today,
                    IsActive = true
                };
                _db.Set<Beneficiary>().Add(newBen);
                await _db.SaveChangesAsync(ct);
                beneficiaryId = newBen.Id;
            }

            // تحقق أنه مش مسجل في المشروع
            var alreadyInProject = await _db.Set<ProjectBeneficiary>()
                .AnyAsync(x => x.ProjectId == vm.ProjectId && x.BeneficiaryId == beneficiaryId, ct);
            if (alreadyInProject)
            {
                TempData["Warning"] = "هذا المستفيد مسجل في المشروع مسبقاً";
                return RedirectToAction(nameof(QuickAddBeneficiary), new { projectId = vm.ProjectId });
            }

            // أضف للمشروع مع الفئة المستهدفة
            _db.Set<ProjectBeneficiary>().Add(new ProjectBeneficiary
            {
                Id = Guid.NewGuid(),
                ProjectId = vm.ProjectId,
                BeneficiaryId = beneficiaryId,
                EnrollmentDate = DateTime.Today,
                BenefitType = vm.BenefitType,
                TargetGroupName = vm.TargetGroupName?.Trim(),
                Notes = vm.Notes
            });
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = $"تم إضافة {vm.FullName} كمستفيد في المشروع بنجاح";
            if (vm.AddAnother) return RedirectToAction(nameof(QuickAddBeneficiary), new { projectId = vm.ProjectId });
            return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
        }

        // ══════════════════════════════════════════════════
        // تحويل المقترح لمشروع
        // ══════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> ConvertProposal(Guid proposalId, CancellationToken ct)
        {
            var proposal = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals.ProjectProposal>()
                .AsNoTracking()
                .Include(p => p.Objectives)
                .Include(p => p.WorkPlanItems)
                .FirstOrDefaultAsync(x => x.Id == proposalId, ct);
            if (proposal == null) return NotFound();
            ViewBag.Proposal = proposal;
            return View(proposal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertProposal(Guid proposalId,
            string projectName, DateTime startDate, DateTime endDate,
            decimal budget, string? location, string? code, CancellationToken ct)
        {
            var proposal = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals.ProjectProposal>()
                .Include(p => p.Objectives)
                .Include(p => p.WorkPlanItems)
                .FirstOrDefaultAsync(x => x.Id == proposalId, ct);
            if (proposal == null) return NotFound();

            // أنشئ المشروع
            var project = new CharityProject
            {
                Id = Guid.NewGuid(),
                Code = code?.Trim() ?? $"P-{DateTime.UtcNow:yyMMdd}",
                Name = projectName.Trim(),
                Description = proposal.ExecutiveSummary ?? proposal.ProposedSolution,
                StartDate = startDate,
                EndDate = endDate,
                Budget = budget,
                Location = location ?? proposal.ProjectLocation,
                Objectives = proposal.GeneralGoal,
                Kpis = proposal.ExpectedResults,
                Notes = !string.IsNullOrWhiteSpace(proposal.SustainabilityPlan) ? $"الاستدامة: {proposal.SustainabilityPlan} المخاطر: {proposal.RisksAndExternalFactors}"


                    : null,
                Status = "Planned",
                IsActive = true
            };
            _db.Set<CharityProject>().Add(project);

            // حوّل الأهداف العامة لهدف رئيسي
            if (!string.IsNullOrWhiteSpace(proposal.GeneralGoal))
            {
                var mainGoal = new ProjectGoal
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    Title = proposal.GeneralGoal,
                    SortOrder = 1,
                    Status = "Active",
                    IsActive = true
                };
                _db.Set<ProjectGoal>().Add(mainGoal);

                // حوّل الأهداف الفرعية من Objectives
                int subOrder = 1;
                foreach (var obj in proposal.Objectives.Where(o => o.ObjectiveType == "Specific"))
                {
                    var subGoal = new ProjectSubGoal
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        GoalId = mainGoal.Id,
                        Title = obj.Title,
                        Description = obj.Description,
                        SortOrder = subOrder++,
                        Status = "Active",
                        IsActive = true
                    };
                    _db.Set<ProjectSubGoal>().Add(subGoal);

                    // أنشطة مرتبطة بهذا الهدف الفرعي من خطة العمل
                    int actOrder = 1;
                    foreach (var wp in proposal.WorkPlanItems.Where(w => w.GoalTitle.Contains(obj.Title[..Math.Min(20, obj.Title.Length)])))
                    {
                        _db.Set<ProjectSubGoalActivity>().Add(new ProjectSubGoalActivity
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = project.Id,
                            SubGoalId = subGoal.Id,
                            Title = wp.ActivityTitle,
                            ResponsiblePersonName = wp.Responsible,
                            SortOrder = actOrder++,
                            Status = "Planned",
                            IsActive = true,
                            Priority = "Medium"
                        });
                    }
                }
            }

            // حدّث حالة المقترح
            proposal.Status = "Approved";
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = $"تم تحويل المقترح «{proposal.Title}» إلى مشروع بنجاح";
            return RedirectToAction(nameof(Index), new { projectId = project.Id });
        }

        private async Task RecalcSubGoalProgressAsync(Guid subGoalId)
        {
            var acts = await _db.Set<ProjectSubGoalActivity>()
                .Where(a => a.SubGoalId == subGoalId && a.IsActive).ToListAsync();
            var sg = await _db.Set<ProjectSubGoal>().FindAsync(subGoalId);
            if (sg != null && acts.Any())
                sg.ProgressPercent = acts.Average(a => a.ProgressPercent);
        }


        // ── ViewModels ──
        // ── يجيب الفئات المستهدفة من المقترح المرتبط بالمشروع ──
        private async Task<List<string>> GetProjectTargetGroupsAsync(Guid projectId)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return new();

            var proposal = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals.ProjectProposal>()
                .AsNoTracking()
                .Include(p => p.TargetGroups)
                .FirstOrDefaultAsync(p => p.ProposalNumber == project.Code);

            if (proposal?.TargetGroups.Any() == true)
                return proposal.TargetGroups
                    .Where(t => !string.IsNullOrWhiteSpace(t.CategoryName))
                    .Select(t => t.CategoryName!).Distinct().ToList();

            // fallback: من الأنشطة
            return await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Projects.ProjectSubGoalActivity>()
                .AsNoTracking()
                .Where(a => a.ProjectId == projectId && !string.IsNullOrEmpty(a.TargetGroup))
                .Select(a => a.TargetGroup!).Distinct().ToListAsync();
        }

        
    }
    public class QuickBeneficiaryVm
    {
        public Guid ProjectId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "الاسم مطلوب")]
        public string FullName { get; set; } = "";
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public int FamilyMembersCount { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? BenefitType { get; set; }
        public string? TargetGroupName { get; set; }   // ← الفئة المستهدفة
        public string? Notes { get; set; }
        public bool AddAnother { get; set; }
        public List<string> TargetGroups { get; set; } = new();  // ← قائمة الفئات من المقترح
    }
}