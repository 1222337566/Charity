using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>
    /// عضو فريق العمل في مشروع — موظف أو متطوع
    /// مربوط بمرحلة ونشاط محدد
    /// </summary>
    public class ProjectTeamMember
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }

        // ── ربط بمرحلة/نشاط (اختياري) ──
        public Guid? PhaseId    { get; set; }
        public Guid? ActivityId { get; set; }

        // ── نوع العضو: Employee | Volunteer ──
        public string MemberType { get; set; } = "Volunteer";
        public Guid?  EmployeeId { get; set; }
        public Guid?  VolunteerId { get; set; }

        // ── الدور والمشاركة ──
        public string? RoleTitle         { get; set; }
        public string  ParticipationStatus { get; set; } = "Assigned";
        // Assigned | Active | Completed | Cancelled | Absent

        // ── التوقيت ──
        public DateTime  StartDate         { get; set; }
        public DateTime? EndDate           { get; set; }
        public decimal   PlannedHours      { get; set; }
        public decimal   ActualHours       { get; set; }
        public int?      PlannedDays       { get; set; }
        public int?      ActualDays        { get; set; }

        // ── إثبات المشاركة ──
        public string  VerificationStatus { get; set; } = "Unverified";
        // Unverified | Verified | Rejected | NeedsReview
        public DateTime? VerificationDate   { get; set; }
        public string?   VerifiedByUserId   { get; set; }
        public string?   VerificationNotes  { get; set; }

        public string?   Notes             { get; set; }
        public string?   CreatedByUserId   { get; set; }
        public DateTime  CreatedAtUtc      { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc      { get; set; }

        // ── Navigation ──
        public CharityProject?         Project    { get; set; }
        public ProjectPhase?           Phase      { get; set; }
        public ProjectSubGoalActivity? Activity   { get; set; }
        public HrEmployee?             Employee   { get; set; }
        public Volunteer?              Volunteer  { get; set; }

        public ICollection<ProjectTeamMemberAttachment> Attachments { get; set; }
            = new List<ProjectTeamMemberAttachment>();
    }
}
