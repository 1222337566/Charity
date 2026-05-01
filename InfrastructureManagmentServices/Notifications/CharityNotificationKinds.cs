namespace InfrastructureManagmentServices.Notification;

public static class CharityNotificationKinds
{
    // ── Existing ──
    public const string AidRequestCreated              = "charity.aid-request.created";
    public const string CommitteeDecisionCreated       = "charity.committee-decision.created";
    public const string GrantInstallmentDueSoon        = "charity.grant-installment.due-soon";
    public const string LowStock                       = "charity.stock.low-balance";
    public const string PayrollApproved                = "charity.payroll.approved";
    public const string MonthlyAidCycleMissing         = "charity.aid-cycle.monthly.missing";
    public const string MonthlyAidCyclePending         = "charity.aid-cycle.monthly.pending";
    public const string MonthlyAidCycleUndisbursed     = "charity.aid-cycle.monthly.undisbursed";
    public const string MonthlyAidCycleOverdue         = "charity.aid-cycle.monthly.overdue";
    public const string MonthlyAidCycleNotIncluded     = "charity.aid-cycle.monthly.not-included";

    // ── Workflow ──
    public const string WorkflowSubmitted              = "charity.workflow.submitted";
    public const string WorkflowApproved               = "charity.workflow.approved";
    public const string WorkflowRejected               = "charity.workflow.rejected";
    public const string WorkflowReturnedForRevision    = "charity.workflow.returned";
    public const string WorkflowEscalated              = "charity.workflow.escalated";

    // ── Kafala ──
    public const string KafalaPaymentDue               = "charity.kafala.payment-due";
    public const string KafalaPaymentOverdue           = "charity.kafala.payment-overdue";
    public const string KafalaCaseSuspended            = "charity.kafala.case-suspended";

    // ── Projects ──
    public const string ProjectPhaseDelayed            = "charity.project.phase-delayed";
    public const string ProjectMilestoneDue            = "charity.project.milestone-due";
    public const string ProjectBudgetExceeded          = "charity.project.budget-exceeded";
    public const string ProjectTrackingLogAdded        = "charity.project.tracking-log";

    // ── Beneficiaries ──
    public const string AidRequestApproved             = "charity.aid-request.approved";
    public const string AidRequestRejected             = "charity.aid-request.rejected";
    public const string AidRequestUnderReview          = "charity.aid-request.under-review";
    public const string HumanitarianResearchDue        = "charity.humanitarian-research.due";
}
