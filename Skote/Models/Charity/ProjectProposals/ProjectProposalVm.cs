using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Skote.Models.Charity.ProjectProposals
{
    public class ProjectProposalVm
    {
        public Guid? Id { get; set; }

        [Display(Name = "رقم المقترح")]
        public string ProposalNumber { get; set; } = string.Empty;

        [Display(Name = "اسم المشروع المقترح")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الجهة المانحة")]     public string? DonorName { get; set; }
        [Display(Name = "اسم المنظمة")]       public string? OrganizationName { get; set; }
        [Display(Name = "موقع المشروع")]      public string? ProjectLocation { get; set; }
        [Display(Name = "تاريخ التقديم")]     public DateTime SubmissionDate { get; set; } = DateTime.Today;
        [Display(Name = "مدة التنفيذ بالشهور")] public int DurationMonths { get; set; }
        [Display(Name = "الميزانية المطلوبة")] public decimal RequestedBudget { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } = "Draft";

        public string? RegistrationNumber { get; set; }
        public int?    RegistrationYear { get; set; }
        public string? Vision { get; set; }
        public string? Mission { get; set; }
        public string? ExpertiseSummary { get; set; }
        public int?    EmployeesCount { get; set; }
        public int?    VolunteersCount { get; set; }
        public int?    YearsOfExperience { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Address { get; set; }
        public string? ProblemBackground { get; set; }
        public string? ProblemAnalysis { get; set; }
        public string? NationalAlignment { get; set; }
        public string? ProposedApproach { get; set; }
        public string? ProposedSolution { get; set; }
        public string? RisksAndExternalFactors { get; set; }
        public string? ExecutiveSummary { get; set; }
        public string? GeneralGoal { get; set; }
        public string? ExpectedResults { get; set; }
        public string? PreparatoryRequirements { get; set; }
        public string? ImplementationTeamSummary { get; set; }
        public string? SustainabilityPlan { get; set; }
        public string? Notes { get; set; }

        public List<ProjectProposalPastExperienceVm>      PastExperiences      { get; set; } = new();
        public List<ProjectProposalTargetGroupVm>          TargetGroups         { get; set; } = new();
        public List<ProjectProposalObjectiveVm>            Objectives           { get; set; } = new();
        public List<ProjectProposalActivityVm>             Activities           { get; set; } = new();
        public List<ProjectProposalWorkPlanVm>             WorkPlanItems        { get; set; } = new();
        public List<ProjectProposalMonitoringIndicatorVm>  MonitoringIndicators { get; set; } = new();
        public List<ProjectProposalTeamMemberVm>           TeamMembers          { get; set; } = new();

        // المراحل — الحقول المباشرة تُستخدم في الـ form
        public List<ProposalPhaseVm>                       Phases               { get; set; } = new();
        // أنشطة المراحل — تُرسل من الـ form كـ PhaseActivities[phaseIndex][actIndex]
        public List<List<ProposalPhaseActivityRowVm>>      PhaseActivities      { get; set; } = new();

        public List<ProjectProposalAttachmentRowVm>        ExistingAttachments  { get; set; } = new();
        public List<IFormFile>?                            NewAttachments       { get; set; }

        /// <summary>true عند الضغط على "حفظ كمسودة" — يتخطى ModelState.IsValid</summary>
        public bool IsDraftSave { get; set; }
    }

    public class ProjectProposalPastExperienceVm
    {
        public string  ProjectName    { get; set; } = string.Empty;
        public string? Donor          { get; set; }
        public string? Location       { get; set; }
        public string? DurationText   { get; set; }
        public decimal? Budget        { get; set; }
        public string? TargetGroup    { get; set; }
        public string? ResultAchieved { get; set; }
    }

    public class ProjectProposalTargetGroupVm
    {
        public string  CategoryName       { get; set; } = string.Empty;
        public int?    TargetCount        { get; set; }
        public string? AgeRange           { get; set; }
        public string? GenderNotes        { get; set; }
        public string? SelectionCriteria  { get; set; }
        public string? BenefitDescription { get; set; }
    }

    public class ProjectProposalObjectiveVm
    {
        public string  ObjectiveType { get; set; } = "Specific";
        public string  Title        { get; set; } = string.Empty;
        public string? Description  { get; set; }
    }

    public class ProjectProposalActivityVm
    {
        public string  Title             { get; set; } = string.Empty;
        public string? Description       { get; set; }   // مؤشر الأداء
        public string? ResponsibleRole   { get; set; }   // مسؤول / عدد الأنشطة (مزدوج في الـ form)
        public string? NeededResources   { get; set; }   // وسائل التحقق
        public int?    PlannedStartMonth { get; set; }   // مدة (أيام) — اسم مُعاد توظيفه
        public int?    PlannedEndMonth   { get; set; }   // ساعات/يوم — اسم مُعاد توظيفه
        public string? Responsible       { get; set; }   // الوحدة (شخص/يوم/...)
        public string? TargetGroup       { get; set; }   // الفئة المستهدفة
        public int?    PlannedCount      { get; set; }   // الكمية المخططة
        public string  Priority          { get; set; } = "Medium";
        public List<ProposalSubActivityVm> SubActivities { get; set; } = new();
    }

    public class ProposalSubActivityVm
    {
        public string  Title       { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int?    Count       { get; set; }
        public string? Unit        { get; set; }
    }

    public class ProjectProposalWorkPlanVm
    {
        public string  GoalTitle     { get; set; } = string.Empty;
        public string  ActivityTitle { get; set; } = string.Empty;
        public string? PhaseName            { get; set; }
        public int     ContributionPercent  { get; set; } = 100;
        public int?    PlannedQuantity      { get; set; }
        public int?    DurationDays         { get; set; }
        public string? Responsible          { get; set; }
        public string? Resources     { get; set; }
        public int?    StartMonth    { get; set; }
        public int?    EndMonth      { get; set; }
    }

    public class ProjectProposalMonitoringIndicatorVm
    {
        public string  ActivityTitle      { get; set; } = string.Empty;
        public string  Indicator          { get; set; } = string.Empty;
        public string? TargetValue        { get; set; }
        public string? AchievedValue      { get; set; }   // تمت الإضافة
        public string? VerificationMethod { get; set; }
        public string? VerificationMeans  { get; set; }   // تمت الإضافة — يُخزن في VerificationMethod
        public string? ReportingFrequency { get; set; }
    }

    public class ProjectProposalTeamMemberVm
    {
        public string  RoleName       { get; set; } = string.Empty;
        public string? Responsibility { get; set; }
        public bool    IsInternal     { get; set; } = true;
    }

    public class ProposalPhaseVm
    {
        public string  Name           { get; set; } = string.Empty;
        public string? Description    { get; set; }
        public int     StartMonth     { get; set; } = 1;
        public int     EndMonth       { get; set; } = 3;
        /// <summary>
        /// عناوين الأنشطة مفصولة بفاصلة — تُستخدم فقط عند عرض الـ form لتعبئة الـ select
        /// البيانات الفعلية عند الحفظ تأتي من PhaseActivities[i][j]
        /// </summary>
        public string  ActivityTitles { get; set; } = string.Empty;
    }

    /// <summary>نشاط مُسكَّن في مرحلة — يأتي من PhaseActivities[phaseIdx][actIdx]</summary>
    public class ProposalPhaseActivityRowVm
    {
        public string  ActivityTitle  { get; set; } = string.Empty;
        public int     ContributionPct { get; set; } = 100;
        public int?    PlannedQuantity { get; set; }
        public int?    DurationDays   { get; set; }
    }

    public class ProjectProposalAttachmentRowVm
    {
        public Guid    Id                { get; set; }
        public string  OriginalFileName  { get; set; } = string.Empty;
        public string? AttachmentType    { get; set; }
        public long    FileSizeBytes     { get; set; }
    }
}
