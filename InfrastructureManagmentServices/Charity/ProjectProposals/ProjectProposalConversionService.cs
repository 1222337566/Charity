using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.ProjectProposals
{
    public class ProjectProposalConversionService : IProjectProposalConversionService
    {
        private readonly AppDbContext _db;
        public ProjectProposalConversionService(AppDbContext db) => _db = db;

        public async Task<CharityProject> ConvertToProjectAsync(
            ProjectProposal proposal, string? approvedByUserId = null)
        {
            var projectStart = DateTime.Today;
            var projectEnd   = projectStart.AddMonths(Math.Max(1, proposal.DurationMonths));

            // ── 1. المشروع الرئيسي ──
            var project = new CharityProject
            {
                Id                       = Guid.NewGuid(),
                Code                     = string.IsNullOrWhiteSpace(proposal.ProposalNumber)
                                             ? $"PRJ-{DateTime.UtcNow:yyyyMMddHHmm}"
                                             : proposal.ProposalNumber,
                Name                     = proposal.Title,
                Description              = proposal.ExecutiveSummary ?? proposal.ProposedSolution,
                StartDate                = projectStart,
                EndDate                  = projectEnd,
                Budget                   = proposal.RequestedBudget,
                Status                   = "Planned",
                TargetBeneficiariesCount = proposal.TargetGroups
                                             .Where(x => x.TargetCount.HasValue)
                                             .Sum(x => x.TargetCount) ?? 0,
                Location     = proposal.ProjectLocation,
                Objectives   = string.Join(Environment.NewLine,
                                 proposal.Objectives.Where(x => x.ObjectiveType == "Specific")
                                                    .Select(x => x.Title)),
                Kpis         = string.Join(Environment.NewLine,
                                 proposal.MonitoringIndicators.Select(x => x.Indicator).Distinct()),
                Notes        = $"تم الإنشاء من المقترح رقم {proposal.ProposalNumber}",
                CreatedAtUtc = DateTime.UtcNow,
                IsActive     = true
            };
            _db.Set<CharityProject>().Add(project);

            // ── 2. الأهداف الرئيسية والفرعية ──
            var generalObjs  = proposal.Objectives.Where(x => x.ObjectiveType == "General")
                                                  .OrderBy(x => x.Id).ToList();
            var specificObjs = proposal.Objectives.Where(x => x.ObjectiveType == "Specific")
                                                  .OrderBy(x => x.Id).ToList();

            var subGoalByTitle = new Dictionary<string, ProjectSubGoal>(StringComparer.OrdinalIgnoreCase);
            ProjectGoal? defaultGoal = null;
            int goalSort = 1;

            foreach (var gObj in generalObjs)
            {
                var goal = new ProjectGoal
                {
                    Id = Guid.NewGuid(), ProjectId = project.Id,
                    SortOrder = goalSort++, Title = gObj.Title, Description = gObj.Description,
                    Status = "Active", IsActive = true, CreatedAtUtc = DateTime.UtcNow
                };
                project.Goals.Add(goal);
                defaultGoal ??= goal;

                int subSort = 1;
                foreach (var sObj in specificObjs)
                {
                    var sub = BuildSubGoal(project.Id, goal.Id, sObj.Title, sObj.Description, subSort++);
                    goal.SubGoals.Add(sub);
                    subGoalByTitle[sub.Title] = sub;
                }
                specificObjs.Clear();
            }

            if (!generalObjs.Any())
            {
                defaultGoal = new ProjectGoal
                {
                    Id = Guid.NewGuid(), ProjectId = project.Id, SortOrder = 1,
                    Title = proposal.GeneralGoal ?? proposal.Title,
                    Description = proposal.ExecutiveSummary,
                    Status = "Active", IsActive = true, CreatedAtUtc = DateTime.UtcNow
                };
                project.Goals.Add(defaultGoal);
                int subSort = 1;
                foreach (var sObj in specificObjs)
                {
                    var sub = BuildSubGoal(project.Id, defaultGoal.Id, sObj.Title, sObj.Description, subSort++);
                    defaultGoal.SubGoals.Add(sub);
                    subGoalByTitle[sub.Title] = sub;
                }
            }

            ProjectSubGoal EnsureDefaultSubGoal()
            {
                if (subGoalByTitle.TryGetValue("__default__", out var ex)) return ex;
                var goal = defaultGoal ?? project.Goals.First();
                var sub  = BuildSubGoal(project.Id, goal.Id, "أنشطة المشروع", null, 999);
                goal.SubGoals.Add(sub);
                subGoalByTitle["__default__"] = sub;
                return sub;
            }

            // ── 3. المراحل من proposal.Phases ──
            var phaseByProposalId = new Dictionary<Guid, ProjectPhase>();
            int phaseSort = 1;
            foreach (var pp in proposal.Phases.OrderBy(x => x.SortOrder))
            {
                var phase = new ProjectPhase
                {
                    Id = Guid.NewGuid(), ProjectId = project.Id,
                    Code = $"PH-{phaseSort:D2}", Name = pp.Name, Description = pp.Description,
                    SortOrder = phaseSort++,
                    PlannedStartDate = projectStart.AddMonths(pp.StartMonth - 1),
                    PlannedEndDate   = projectStart.AddMonths(pp.EndMonth   - 1),
                    Status = "Planned", ProgressPercent = 0, PlannedCost = 0, ActualCost = 0,
                    IsActive = true, CreatedAtUtc = DateTime.UtcNow
                };
                _db.Set<ProjectPhase>().Add(phase);
                phaseByProposalId[pp.Id] = phase;
            }

            // ── 4. الأنشطة من proposal.Activities → ProjectSubGoalActivity ──
            //
            // ═══ wrkplans (WorkPlanItems) ═══
            // WorkPlanItems هي "خطة العمل الإرشادية" — تحتوي على:
            //   ActivityTitle, PhaseName, GoalTitle, StartMonth, EndMonth,
            //   ContributionPercent, PlannedQuantity, DurationDays, Responsible
            //
            // نبني lookup متعدد الأبعاد:
            //   - workPlanByTitle:   ActivityTitle → first WP item (للشهور)
            //   - workPlansByPhase:  PhaseName     → List<WP items> (للـ phaseAssignments)

            var workPlanByTitle = proposal.WorkPlanItems
                .Where(w => !string.IsNullOrWhiteSpace(w.ActivityTitle))
                .GroupBy(w => w.ActivityTitle!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var workPlansByActivity = proposal.WorkPlanItems
                .Where(w => !string.IsNullOrWhiteSpace(w.ActivityTitle))
                .GroupBy(w => w.ActivityTitle!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // خريطة PhaseName → ProjectPhase (من proposal.Phases)
            var phaseByName = proposal.Phases
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var activityByTitle = new Dictionary<string, ProjectSubGoalActivity>(StringComparer.OrdinalIgnoreCase);
            int actSort = 1;

            foreach (var pa in proposal.Activities.OrderBy(x => x.SortOrder))
            {
                var subGoal = subGoalByTitle.Values
                    .FirstOrDefault(sg =>
                        sg.Title.Contains(pa.Title, StringComparison.OrdinalIgnoreCase) ||
                        pa.Title.Contains(sg.Title, StringComparison.OrdinalIgnoreCase))
                    ?? subGoalByTitle.Values.FirstOrDefault()
                    ?? EnsureDefaultSubGoal();

                DateTime? actStart = pa.PlannedStartMonth.HasValue
                    ? projectStart.AddMonths(pa.PlannedStartMonth.Value - 1) : null;
                DateTime? actEnd   = pa.PlannedEndMonth.HasValue
                    ? projectStart.AddMonths(pa.PlannedEndMonth.Value - 1) : null;

                // أكمل الشهور من wrkplans لو ناقصة
                if (workPlanByTitle.TryGetValue(pa.Title, out var wp))
                {
                    actStart ??= wp.StartMonth.HasValue ? projectStart.AddMonths(wp.StartMonth.Value - 1) : null;
                    actEnd   ??= wp.EndMonth.HasValue   ? projectStart.AddMonths(wp.EndMonth.Value - 1)   : null;
                }

                var act = new ProjectSubGoalActivity
                {
                    Id = Guid.NewGuid(), ProjectId = project.Id, SubGoalId = subGoal.Id,
                    SortOrder = actSort++, Title = pa.Title, Description = pa.Description,
                    ResponsiblePersonName = pa.Responsible ?? pa.ResponsibleRole,
                    PlannedStartDate = actStart, PlannedEndDate = actEnd,
                    Status = "Planned", Priority = pa.Priority ?? "Medium",
                    ProgressPercent = 0, PlannedCost = 0,
                    PlannedHours = pa.PlannedHoursPerDay.HasValue && pa.PlannedDurationDays.HasValue
                                    ? (decimal)(pa.PlannedHoursPerDay.Value * pa.PlannedDurationDays.Value) : 0,
                    TargetGroup         = pa.TargetGroup ?? string.Empty,
                    PlannedQuantity     = pa.PlannedCount ?? 0,
                    PlannedDurationDays = pa.PlannedDurationDays ?? 0,
                    QuantityUnit        = pa.QuantityUnit ?? string.Empty,
                    PlannedHoursPerDay  = (int)(pa.PlannedHoursPerDay ?? 0),
                    IsActive = true, CreatedAtUtc = DateTime.UtcNow,
                    PhaseAssignments = new List<ActivityPhaseAssignment>()
                };
                subGoal.Activities.Add(act);
                activityByTitle[act.Title] = act;
            }

            // ── 5. WorkPlan items ليس لها نشاط مقابل ──
            foreach (var wpItem in proposal.WorkPlanItems)
            {
                var titleKey = wpItem.ActivityTitle?.Trim() ?? "";
                if (string.IsNullOrEmpty(titleKey) || activityByTitle.ContainsKey(titleKey)) continue;

                var subGoal = (wpItem.GoalTitle != null && subGoalByTitle.TryGetValue(wpItem.GoalTitle, out var sg))
                    ? sg : subGoalByTitle.Values.FirstOrDefault() ?? EnsureDefaultSubGoal();

                var act = new ProjectSubGoalActivity
                {
                    Id = Guid.NewGuid(), ProjectId = project.Id, SubGoalId = subGoal.Id,
                    SortOrder = actSort++, Title = titleKey,
                    ResponsiblePersonName = wpItem.Responsible,
                    PlannedStartDate = wpItem.StartMonth.HasValue ? projectStart.AddMonths(wpItem.StartMonth.Value - 1) : null,
                    PlannedEndDate   = wpItem.EndMonth.HasValue   ? projectStart.AddMonths(wpItem.EndMonth.Value   - 1) : null,
                    Status = "Planned", Priority = "Medium",
                    TargetGroup = string.Empty, QuantityUnit = string.Empty,
                    IsActive = true, CreatedAtUtc = DateTime.UtcNow,
                    PhaseAssignments = new List<ActivityPhaseAssignment>()
                };
                subGoal.Activities.Add(act);
                activityByTitle[act.Title] = act;
            }

            // ── 6. ActivityPhaseAssignment من wrkplans ──────────────────────────
            //
            // الاستراتيجية:
            //   أ) من proposal.Phases.Activities (التسكين المباشر من الـ form)
            //   ب) من WorkPlanItems التي تحتوي PhaseName (خطة العمل الإرشادية)
            //   ج) منع التكرار: نفس Activity × Phase

            var assignmentKeys = new HashSet<string>(); // "activityId|phaseId"

            void AddAssignment(ProjectSubGoalActivity act, ProjectPhase phase,
                               int sortOrder, decimal contribution, decimal qty, int? days)
            {
                var key = $"{act.Id}|{phase.Id}";
                if (!assignmentKeys.Add(key)) return; // منع التكرار

                act.PhaseAssignments.Add(new ActivityPhaseAssignment
                {
                    Id                  = Guid.NewGuid(),
                    ActivityId          = act.Id,
                    PhaseId             = phase.Id,
                    SortOrder           = sortOrder,
                    ContributionPercent = contribution,
                    PlannedQuantity     = qty,
                    PlannedDurationDays = days,
                    PlannedStartDate    = phase.PlannedStartDate,
                    PlannedEndDate      = phase.PlannedEndDate,
                    Status              = "Planned",
                    PlannedCost         = 0,
                    CreatedAtUtc        = DateTime.UtcNow
                });
            }

            // أ) من Phases.Activities (proposal form sections)
            foreach (var pp in proposal.Phases.OrderBy(x => x.SortOrder))
            {
                if (!phaseByProposalId.TryGetValue(pp.Id, out var phase)) continue;
                int assignSort = 1;
                foreach (var ppa in pp.Activities.OrderBy(x => x.SortOrder))
                {
                    if (!activityByTitle.TryGetValue(ppa.ActivityTitle, out var act)) continue;
                    AddAssignment(act, phase, assignSort++, ppa.ContributionPercent,
                                  ppa.PlannedQuantity ?? 0, ppa.DurationDays);
                }
            }

            // ب) من WorkPlanItems.PhaseName (wrkplans)
            foreach (var wpItem in proposal.WorkPlanItems
                         .Where(w => !string.IsNullOrWhiteSpace(w.PhaseName)
                                  && !string.IsNullOrWhiteSpace(w.ActivityTitle)))
            {
                if (!activityByTitle.TryGetValue(wpItem.ActivityTitle!, out var act)) continue;
                if (!phaseByName.TryGetValue(wpItem.PhaseName!, out var proposalPhase)) continue;
                if (!phaseByProposalId.TryGetValue(proposalPhase.Id, out var phase)) continue;

                AddAssignment(act, phase,
                    sortOrder:    act.PhaseAssignments.Count + 1,
                    contribution: wpItem.ContributionPercent,
                    qty:          wpItem.PlannedQuantity ?? 0,
                    days:         wpItem.DurationDays);
            }

            // ── 7. تحديث حالة المقترح ──
            proposal.CharityProjectId = project.Id;
            proposal.Status           = "ConvertedToProject";
            proposal.ApprovedByUserId = approvedByUserId;
            proposal.ApprovedAtUtc    = DateTime.UtcNow;
            proposal.UpdatedAtUtc     = DateTime.UtcNow;
            _db.Set<ProjectProposal>().Update(proposal);

            await _db.SaveChangesAsync();
            return project;
        }

        // ── SyncFromWorkPlan — تحديث ActivityPhaseAssignments من wrkplans دون تكرار ──
        public async Task SyncWorkPlanToProjectAsync(Guid projectId, Guid proposalId)
        {
            var proposal = await _db.Set<ProjectProposal>()
                .Include(p => p.WorkPlanItems)
                .Include(p => p.Phases).ThenInclude(ph => ph.Activities)
                .FirstOrDefaultAsync(p => p.Id == proposalId);
            if (proposal == null) return;

            var project = await _db.Set<CharityProject>().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return;

            var projectStart = project.StartDate;

            // تحميل الأنشطة الموجودة في المشروع
            var existingActivities = await _db.Set<ProjectSubGoalActivity>()
                .Include(a => a.PhaseAssignments)
                .Where(a => a.ProjectId == projectId)
                .ToListAsync();

            var activityByTitle = existingActivities
                .GroupBy(a => a.Title, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            // تحميل المراحل الموجودة في المشروع
            var projectPhases = await _db.Set<ProjectPhase>()
                .Where(ph => ph.ProjectId == projectId)
                .ToListAsync();
            var phaseByName = projectPhases
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var existingKeys = existingActivities
                .SelectMany(a => a.PhaseAssignments.Select(pa => $"{a.Id}|{pa.PhaseId}"))
                .ToHashSet();

            foreach (var wpItem in proposal.WorkPlanItems
                         .Where(w => !string.IsNullOrWhiteSpace(w.PhaseName)
                                  && !string.IsNullOrWhiteSpace(w.ActivityTitle)))
            {
                if (!activityByTitle.TryGetValue(wpItem.ActivityTitle!, out var act)) continue;
                if (!phaseByName.TryGetValue(wpItem.PhaseName!, out var phase)) continue;

                var key = $"{act.Id}|{phase.Id}";
                if (existingKeys.Contains(key))
                {
                    // تحديث الـ assignment الموجود بدل التكرار
                    var existing = act.PhaseAssignments.FirstOrDefault(pa => pa.PhaseId == phase.Id);
                    if (existing != null)
                    {
                        existing.ContributionPercent = wpItem.ContributionPercent;
                        existing.PlannedQuantity     = wpItem.PlannedQuantity ?? 0;
                        existing.PlannedDurationDays = wpItem.DurationDays;
                        existing.PlannedStartDate    = wpItem.StartMonth.HasValue
                            ? projectStart.AddMonths(wpItem.StartMonth.Value - 1) : phase.PlannedStartDate;
                        existing.PlannedEndDate      = wpItem.EndMonth.HasValue
                            ? projectStart.AddMonths(wpItem.EndMonth.Value - 1)   : phase.PlannedEndDate;
                        _db.Set<ActivityPhaseAssignment>().Update(existing);
                    }
                }
                else
                {
                    // إضافة assignment جديد
                    var newAssign = new ActivityPhaseAssignment
                    {
                        Id                  = Guid.NewGuid(),
                        ActivityId          = act.Id,
                        PhaseId             = phase.Id,
                        SortOrder           = act.PhaseAssignments.Count + 1,
                        ContributionPercent = wpItem.ContributionPercent,
                        PlannedQuantity     = wpItem.PlannedQuantity ?? 0,
                        PlannedDurationDays = wpItem.DurationDays,
                        PlannedStartDate    = wpItem.StartMonth.HasValue
                            ? projectStart.AddMonths(wpItem.StartMonth.Value - 1) : phase.PlannedStartDate,
                        PlannedEndDate      = wpItem.EndMonth.HasValue
                            ? projectStart.AddMonths(wpItem.EndMonth.Value - 1)   : phase.PlannedEndDate,
                        Status       = "Planned",
                        PlannedCost  = 0,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    _db.Set<ActivityPhaseAssignment>().Add(newAssign);
                    existingKeys.Add(key);
                }
            }

            await _db.SaveChangesAsync();
        }

        private static ProjectSubGoal BuildSubGoal(
            Guid projectId, Guid goalId, string title, string? description, int sortOrder) => new()
        {
            Id = Guid.NewGuid(), ProjectId = projectId, GoalId = goalId,
            SortOrder = sortOrder, Title = title, Description = description,
            Status = "Active", IsActive = true, CreatedAtUtc = DateTime.UtcNow
        };
    }
}
