namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrLeaveType
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string Category { get; set; } = "Annual"; // Annual|Sick|Emergency|Unpaid|Maternity|Paternity|Hajj|Other
        public int MaxDaysPerYear { get; set; }           // الحد الأقصى سنوياً
        public int MaxConsecutiveDays { get; set; } = 30; // الحد الأقصى متصلة
        public bool RequiresAttachment { get; set; }       // تتطلب مستند طبي
        public bool PaidLeave { get; set; } = true;
        public bool CarryOverAllowed { get; set; }         // يسمح بترحيل الرصيد
        public int MaxCarryOverDays { get; set; }
        public string Color { get; set; } = "#0e6f73";    // للتقويم
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
