// Re-export from Services project so existing Skote code keeps working
namespace Skote.Helpers;

public static class CharityNotificationKinds
{
    public const string AidRequestCreated           = InfrastructureManagmentServices.Notification.CharityNotificationKinds.AidRequestCreated;
    public const string CommitteeDecisionCreated    = InfrastructureManagmentServices.Notification.CharityNotificationKinds.CommitteeDecisionCreated;
    public const string GrantInstallmentDueSoon     = InfrastructureManagmentServices.Notification.CharityNotificationKinds.GrantInstallmentDueSoon;
    public const string LowStock                    = InfrastructureManagmentServices.Notification.CharityNotificationKinds.LowStock;
    public const string PayrollApproved             = InfrastructureManagmentServices.Notification.CharityNotificationKinds.PayrollApproved;
    public const string MonthlyAidCycleMissing      = InfrastructureManagmentServices.Notification.CharityNotificationKinds.MonthlyAidCycleMissing;
    public const string MonthlyAidCyclePending      = InfrastructureManagmentServices.Notification.CharityNotificationKinds.MonthlyAidCyclePending;
    public const string MonthlyAidCycleUndisbursed  = InfrastructureManagmentServices.Notification.CharityNotificationKinds.MonthlyAidCycleUndisbursed;
    public const string MonthlyAidCycleOverdue      = InfrastructureManagmentServices.Notification.CharityNotificationKinds.MonthlyAidCycleOverdue;
    public const string MonthlyAidCycleNotIncluded  = InfrastructureManagmentServices.Notification.CharityNotificationKinds.MonthlyAidCycleNotIncluded;
    public const string WorkflowSubmitted           = InfrastructureManagmentServices.Notification.CharityNotificationKinds.WorkflowSubmitted;
    public const string WorkflowApproved            = InfrastructureManagmentServices.Notification.CharityNotificationKinds.WorkflowApproved;
    public const string WorkflowRejected            = InfrastructureManagmentServices.Notification.CharityNotificationKinds.WorkflowRejected;
    public const string WorkflowReturnedForRevision = InfrastructureManagmentServices.Notification.CharityNotificationKinds.WorkflowReturnedForRevision;
}
