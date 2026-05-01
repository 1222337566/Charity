namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>
    /// ربط نشاط بمرحلة — مع كميات مخططة وفعلية لكل مرحلة
    /// نشاط واحد ممكن يكون في عدة مراحل بكميات مختلفة
    /// </summary>
    public class ActivityPhaseAssignment
    {
        public Guid Id { get; set; }

        public Guid ActivityId { get; set; }
        public ProjectSubGoalActivity? Activity { get; set; }

        public Guid PhaseId { get; set; }
        public ProjectPhase? Phase { get; set; }

        public int SortOrder { get; set; } = 1;

        // ── الكميات في هذه المرحلة تحديداً ──
        public decimal PlannedQuantity { get; set; }     // كمية مخططة في هذه المرحلة
        public decimal ActualQuantity  { get; set; }     // كمية فعلية منفذة

        // ── نسبة مساهمة هذه المرحلة من إجمالي النشاط (100% لو مرحلة واحدة) ──
        public decimal ContributionPercent { get; set; } = 100;

        // ── المدة في هذه المرحلة ──
        public int? PlannedDurationDays { get; set; }
        public int? ActualDurationDays  { get; set; }
        public decimal? PlannedHoursPerDay { get; set; }

        // ── التواريخ في هذه المرحلة ──
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate   { get; set; }
        public DateTime? ActualStartDate  { get; set; }
        public DateTime? ActualEndDate    { get; set; }

        // ── التقدم ──
        public decimal ProgressPercent { get; set; }
        public string  Status { get; set; } = "Planned"; // Planned|InProgress|Completed

        // ── التكلفة في هذه المرحلة ──
        public decimal PlannedCost { get; set; }
        public decimal ActualCost  { get; set; }

        public string?   Notes        { get; set; }
        public DateTime  CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
