using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Notification;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Workflow
{
    public class WorkflowService : IWorkflowService
    {
        private readonly AppDbContext _db;
        private readonly ICharityOperationNotificationService _notify;

        public WorkflowService(AppDbContext db, ICharityOperationNotificationService notify)
        {
            _db = db;
            _notify = notify;
        }

        public async Task<List<WorkflowStep>> InitiateAsync(
            string entityType, Guid entityId, string entityTitle,
            string submittedByUserId, CancellationToken ct = default)
        {
            // أزل خطوات قديمة لو وُجدت بدون تحميلها كاملة في الذاكرة
            await _db.Set<WorkflowStep>()
                .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                .ExecuteDeleteAsync(ct);

            // var templates = WorkflowDefinition.GetSteps(entityType);
            var dynamicDef = await _db.Set<DynamicWorkflowDefinition>()
     .AsNoTracking()
     .Include(x => x.Steps.OrderBy(s => s.StepOrder))
     .FirstOrDefaultAsync(x => x.EntityType == entityType && x.IsActive, ct);

            var templates = dynamicDef != null
                ? dynamicDef.Steps.Select(s => new WorkflowStepTemplate(s.StepOrder, s.StepName, s.AssignedRole)).ToList()
                : WorkflowDefinition.GetSteps(entityType);
            var steps = templates.Select(t => new WorkflowStep
            {
                EntityType = entityType,
                EntityId = entityId,
                EntityTitle = entityTitle,
                StepOrder = t.Order,
                StepName = t.Name,
                AssignedRole = t.Role,
                Status = t.Order == 1 ? "Pending" : "Waiting",
                IsActive = true
            }).ToList();

            await _db.Set<WorkflowStep>().AddRangeAsync(steps, ct);
            await _db.SaveChangesAsync(ct);

            var first = steps.FirstOrDefault();
            if (first != null)
            {
                await _notify.NotifyCustomFastAsync(
                    title: $"طلب جديد يحتاج مراجعتك",
                    message: $"يوجد {EntityTypeAr(entityType)} جديد «{entityTitle}» في انتظار: {first.StepName}",
                    kind: CharityNotificationKinds.WorkflowSubmitted,
                    level: "info",
                    actorUserId: submittedByUserId,
                    roleNames: new[] { first.AssignedRole },
                    ct: ct);
            }

            return steps;
        }

        public async Task<WorkflowStep?> GetCurrentStepAsync(
            string entityType, Guid entityId, CancellationToken ct = default)
            => await _db.Set<WorkflowStep>()
                .AsNoTracking()
                .Where(x => x.EntityType == entityType && x.EntityId == entityId
                         && x.Status == "Pending" && x.IsActive)
                .OrderBy(x => x.StepOrder)
                .FirstOrDefaultAsync(ct);

        public async Task<List<WorkflowStep>> GetStepsAsync(
            string entityType, Guid entityId, CancellationToken ct = default)
            => await _db.Set<WorkflowStep>()
                .AsNoTracking()
                .Where(x => x.EntityType == entityType && x.EntityId == entityId && x.IsActive)
                .OrderBy(x => x.StepOrder)
                .ToListAsync(ct);

        public async Task<WorkflowActionResult> TakeActionAsync(
            Guid stepId, string action, string actorUserId,
            string? note = null, CancellationToken ct = default)
        {
            var normalizedAction = NormalizeAction(action);

            var step = await _db.Set<WorkflowStep>()
                .FirstOrDefaultAsync(x => x.Id == stepId && x.IsActive, ct);

            if (step == null)
                return new(false, false, false, null, "الخطوة غير موجودة");

            if (!string.Equals(step.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                return new(false, false, false, null, "هذه الخطوة لم تعد معلقة أو تم اتخاذ إجراء عليها بالفعل");

            step.Status = normalizedAction;
            step.ActionByUserId = actorUserId;
            step.ActionDate = DateTime.UtcNow;
            step.ActionNote = note?.Trim();

            if (normalizedAction == "Rejected")
            {
                await _db.Set<WorkflowStep>()
                    .Where(x => x.EntityType == step.EntityType
                             && x.EntityId == step.EntityId
                             && x.StepOrder > step.StepOrder
                             && x.IsActive
                             && x.Status != "Skipped")
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, "Skipped"), ct);

                await _db.SaveChangesAsync(ct);

                await _notify.NotifyCustomFastAsync(
                    $"تم رفض {EntityTypeAr(step.EntityType)}",
                    $"«{step.EntityTitle}» — {step.StepName}: {note ?? "—"}",
                    CharityNotificationKinds.WorkflowRejected, "error",
                    actorUserId, new[] { CharityNotificationAudience.CharityManager }, ct: ct);

                return new(true, false, true, null, "تم الرفض", step.EntityType, step.EntityId);
            }

            if (normalizedAction == "Returned")
            {
                await _db.SaveChangesAsync(ct);

                var returnNote = string.IsNullOrWhiteSpace(note)
                    ? string.Empty
                    : $" - {note.Trim()}";

                await _notify.NotifyCustomFastAsync(
                    $"إرجاع {EntityTypeAr(step.EntityType)} للمراجعة",
                    $"تم إرجاع «{step.EntityTitle}» من خطوة «{step.StepName}»{returnNote}",
                    CharityNotificationKinds.WorkflowReturnedForRevision,
                    "Warning",
                    actorUserId,
                    new[] { CharityNotificationAudience.BeneficiariesOfficer },
                    ct: ct);

                return new(true, false, false, null, "تمت الإعادة للمراجعة", step.EntityType, step.EntityId);
            }
            if (normalizedAction != "Approved")
                return new(false, false, false, null, "إجراء غير معروف");

            var next = await _db.Set<WorkflowStep>()
                .Where(x => x.EntityType == step.EntityType
                         && x.EntityId == step.EntityId
                         && x.StepOrder > step.StepOrder
                         && x.IsActive
                         && x.Status != "Skipped")
                .OrderBy(x => x.StepOrder)
                .FirstOrDefaultAsync(ct);

            if (next != null)
            {
                next.Status = "Pending";
                await _db.SaveChangesAsync(ct);

                await _notify.NotifyCustomFastAsync(
                    $"خطوة جديدة تحتاج موافقتك",
                    $"«{step.EntityTitle}» وصل لمرحلة: {next.StepName}",
                    CharityNotificationKinds.WorkflowSubmitted, "info",
                    actorUserId, new[] { next.AssignedRole }, ct: ct);

                return new(true, false, false, next, "انتقل للخطوة التالية", step.EntityType, step.EntityId);
            }

            await _db.SaveChangesAsync(ct);

            await _notify.NotifyCustomFastAsync(
                $"اكتمل {EntityTypeAr(step.EntityType)} باعتماده",
                $"«{step.EntityTitle}» اجتاز جميع خطوات الموافقة بنجاح",
                CharityNotificationKinds.WorkflowApproved, "success",
                actorUserId,
                new[] { CharityNotificationAudience.CharityManager, CharityNotificationAudience.FinancialOfficer },
                ct: ct);

            return new(true, true, false, null, "اكتمل المسار — تمت الموافقة", step.EntityType, step.EntityId);
        }

        public async Task<List<WorkflowStep>> GetPendingForRoleAsync(
            string roleName, int take = 50, CancellationToken ct = default)
            => await GetPendingForRolesAsync(new[] { roleName }, take, ct);

        public async Task<List<WorkflowStep>> GetPendingForRolesAsync(
            IEnumerable<string> roleNames, int take = 100, CancellationToken ct = default)
        {
            var roles = roleNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (roles.Count == 0)
                return new List<WorkflowStep>();

            return await _db.Set<WorkflowStep>()
                .AsNoTracking()
                .Where(x => roles.Contains(x.AssignedRole)
                         && x.Status == "Pending"
                         && x.IsActive)
                .OrderBy(x => x.CreatedAtUtc)
                .ThenBy(x => x.StepOrder)
                .Take(take)
                .ToListAsync(ct);
        }

        private static string NormalizeAction(string? action)
        {
            var value = (action ?? string.Empty).Trim();
            return value switch
            {
                "Approve" or "Approved" => "Approved",
                "Reject" or "Rejected" => "Rejected",
                "Return" or "Returned" => "Returned",
                _ => value
            };
        }

        private static string EntityTypeAr(string t) => t switch
        {
            "AidRequest" => "طلب مساعدة",
            "HumanitarianResearch" => "البحث الإنساني",
            "KafalaCase" => "كفالة",
            "AidCycle" => "دورة صرف",
            "ProjectPhase"    => "مرحلة مشروع",
            "ProjectProposal" => "مقترح مشروع",
            _ => t
        };
    }
}
