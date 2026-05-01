namespace Skote.Helpers;

public static class CharityPermissions
{
    public const string Type = "permission";

    public const string DashboardView = "charity.dashboard.view";
    public const string BeneficiariesView = "charity.beneficiaries.view";
    public const string BeneficiariesManage = "charity.beneficiaries.manage";
    public const string ResearchReview = "charity.research.review";
    public const string CommitteeDecisionManage = "charity.committee.manage";
    public const string AidDisbursementView = "charity.aid-disbursement.view";
    public const string AidDisbursementManage = "charity.aid-disbursement.manage";
    public const string DonorsView = "charity.donors.view";
    public const string DonorsManage = "charity.donors.manage";
    public const string FundersView = "charity.funders.view";
    public const string FundersManage = "charity.funders.manage";
    public const string ProjectsView = "charity.projects.view";
    public const string ProjectsManage = "charity.projects.manage";
    public const string StoresView = "charity.stores.view";
    public const string StoresManage = "charity.stores.manage";
    public const string StoresApprove = "charity.stores.approve";
    public const string HrView = "charity.hr.view";
    public const string HrManage = "charity.hr.manage";
    public const string PayrollView = "charity.payroll.view";
    public const string PayrollManage = "charity.payroll.manage";
    public const string VolunteersView = "charity.volunteers.view";
    public const string VolunteersManage = "charity.volunteers.manage";
    public const string ProcurementView = "charity.procurement.view";
    public const string ProcurementManage = "charity.procurement.manage";
    public const string SalesView = "charity.sales.view";
    public const string SalesManage = "charity.sales.manage";
    public const string FinanceView = "charity.finance.view";
    public const string FinanceManage = "charity.finance.manage";
    public const string MinutesView = "charity.minutes.view";
    public const string MinutesManage = "charity.minutes.manage";
    public const string ReportsView = "charity.reports.view";
    public const string AuditView = "charity.audit.view";
    public const string SettingsManage = "charity.settings.manage";

    public static readonly string[] All =
    {
        DashboardView,
        BeneficiariesView,
        BeneficiariesManage,
        ResearchReview,
        CommitteeDecisionManage,
        AidDisbursementView,
        AidDisbursementManage,
        DonorsView,
        DonorsManage,
        FundersView,
        FundersManage,
        ProjectsView,
        ProjectsManage,
        StoresView,
        StoresManage,
        StoresApprove,
        HrView,
        HrManage,
        PayrollView,
        PayrollManage,
        VolunteersView,
        VolunteersManage,
        ProcurementView,
        ProcurementManage,
        SalesView,
        SalesManage,
        FinanceView,
        FinanceManage,
        MinutesView,
        MinutesManage,
        ReportsView,
        AuditView,
        SettingsManage
    };
}
