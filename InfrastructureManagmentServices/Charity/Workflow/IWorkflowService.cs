using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;

namespace InfrastructureManagmentServices.Charity.Workflow
{
    public interface IWorkflowService
    {
        /// <summary>تهيئة خطوات مسار الموافقة لطلب جديد</summary>
        Task<List<WorkflowStep>> InitiateAsync(
            string entityType, Guid entityId, string entityTitle,
            string submittedByUserId, CancellationToken ct = default);

        /// <summary>الخطوة الحالية المعلقة</summary>
        Task<WorkflowStep?> GetCurrentStepAsync(
            string entityType, Guid entityId, CancellationToken ct = default);

        /// <summary>كل خطوات الطلب</summary>
        Task<List<WorkflowStep>> GetStepsAsync(
            string entityType, Guid entityId, CancellationToken ct = default);

        /// <summary>اتخاذ إجراء (موافقة / رفض / إعادة)</summary>
        Task<WorkflowActionResult> TakeActionAsync(
            Guid stepId, string action, string actorUserId,
            string? note = null, CancellationToken ct = default);

        /// <summary>الطلبات المعلقة لدور معين</summary>
        Task<List<WorkflowStep>> GetPendingForRoleAsync(
            string roleName, int take = 50, CancellationToken ct = default);

        /// <summary>الطلبات المعلقة لمجموعة أدوار في استعلام واحد لتسريع لوحة سير العمل</summary>
        Task<List<WorkflowStep>> GetPendingForRolesAsync(
            IEnumerable<string> roleNames, int take = 100, CancellationToken ct = default);
    }

    public record WorkflowActionResult(
        bool Success,
        bool WorkflowCompleted,
        bool WorkflowRejected,
        WorkflowStep? NextStep,
        string Message,
        string? EntityType = null,
        Guid? EntityId = null);
}
