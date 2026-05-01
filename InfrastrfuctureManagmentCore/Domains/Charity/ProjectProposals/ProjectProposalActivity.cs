using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalActivity
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }        // مؤشر الأداء
        public string? ResponsibleRole { get; set; }    // المسؤول
        public string? NeededResources { get; set; }    // وسائل التحقق
        public string? TargetGroup { get; set; }        // الفئة المستهدفة
        public int? PlannedCount { get; set; }          // الكمية المخططة
        public string? QuantityUnit { get; set; }       // الوحدة (شخص/يوم/...)
        public int? PlannedDurationDays { get; set; }   // مدة بالأيام
        public decimal? PlannedHoursPerDay { get; set; }// ساعات/يوم
        public int? ActivityCount { get; set; }         // عدد الأنشطة
        public string? Responsible { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? PlannedStartMonth { get; set; }
        public int? PlannedEndMonth { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
