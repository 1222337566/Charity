namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    /// <summary>
    /// سجل التواصل مع المستفيد — مكالمة / زيارة / رسالة / بريد
    /// </summary>
    public class BeneficiaryContactLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        // نوع التواصل: PhoneCall | Visit | WhatsApp | Email | Other
        public string ContactType { get; set; } = "PhoneCall";

        public DateTime ContactDate { get; set; } = DateTime.Today;

        // نتيجة التواصل: Reached | NotReached | Pending | FollowUp
        public string Outcome { get; set; } = "Reached";

        public string? Subject { get; set; }    // موضوع التواصل
        public string? Notes { get; set; }       // تفاصيل المحادثة

        public DateTime? FollowUpDate { get; set; }  // تاريخ متابعة مقرر
        public string? FollowUpNote { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
