namespace InfrastrfuctureManagmentCore.Domains.Charity.Workflow
{
    /// <summary>
    /// خطوة واحدة في مسار موافقة — تُستخدم لأي طلب (مساعدة، كفالة، دورة، مشروع)
    /// </summary>
    public class WorkflowStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // المرجع
        public string EntityType   { get; set; } = string.Empty; // AidRequest | KafalaCase | AidCycle | ProjectPhase
        public Guid   EntityId     { get; set; }
        public string EntityTitle  { get; set; } = string.Empty; // نص وصفي للعرض

        // الخطوة
        public int    StepOrder    { get; set; }
        public string StepName     { get; set; } = string.Empty; // "مراجعة أولية" | "لجنة" | "اعتماد مالي"
        public string Status       { get; set; } = "Pending";    // Pending | Approved | Rejected | Returned | Skipped
        public string AssignedRole { get; set; } = string.Empty; // الدور المسؤول
        public string? AssignedUserId { get; set; }

        public string? ActionByUserId { get; set; }
        public DateTime? ActionDate   { get; set; }
        public string? ActionNote     { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
