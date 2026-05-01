namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>
    /// ربط مستفيد بنشاط محدد داخل مرحلة — مع حالة الإثبات والمرفقات
    /// </summary>
    public class ProjectActivityBeneficiary
    {
        public Guid Id { get; set; }

        // ── الروابط ──
        public Guid ProjectId        { get; set; }
        public Guid PhaseId          { get; set; }
        public Guid ActivityId       { get; set; }   // ProjectSubGoalActivity
        public Guid BeneficiaryId    { get; set; }   // ProjectBeneficiary.Id

        // ── تصنيف الفئة المستهدفة (من المقترح) ──
        public string? TargetGroupName { get; set; }

        // ── نوع العلاقة ──
        /// <summary>Participant | Beneficiary</summary>
        public string ParticipationType { get; set; } = "Beneficiary";

        // ── إثبات المشاركة ──
        /// <summary>Unverified | Verified | Rejected | NeedsReview</summary>
        public string VerificationStatus { get; set; } = "Unverified";
        public DateTime? VerificationDate   { get; set; }
        public string?   VerifiedByUserId   { get; set; }
        public string?   VerificationNotes  { get; set; }

        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        // ── Navigation ──
        public CharityProject?          Project    { get; set; }
        public ProjectPhase?            Phase      { get; set; }
        public ProjectSubGoalActivity?  Activity   { get; set; }
        public ProjectBeneficiary?      Beneficiary { get; set; }

        public ICollection<ProjectActivityBeneficiaryAttachment> Attachments { get; set; }
            = new List<ProjectActivityBeneficiaryAttachment>();
    }
}
