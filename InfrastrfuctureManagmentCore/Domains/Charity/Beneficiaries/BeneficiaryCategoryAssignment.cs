using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryCategoryAssignment
    {
        public Guid Id { get; set; }

        public Guid BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public BeneficiaryCategory Category { get; set; } = null!;

        // مرحلة أولى بدون FK لتجنب كسر المشروع إذا كانت أسماء كيانات المشروع/النشاط مختلفة.
        public Guid? ProjectId { get; set; }

        public Guid? ProjectActivityId { get; set; }

        public string Status { get; set; } = BeneficiaryCategoryAssignmentStatuses.Waiting;

        public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }
    }

    public static class BeneficiaryCategoryAssignmentStatuses
    {
        public const string Waiting = "Waiting";
        public const string Accepted = "Accepted";
        public const string Participant = "Participant";
        public const string Served = "Served";
        public const string Cancelled = "Cancelled";

        public static string ToArabic(string? status)
        {
            return status switch
            {
                Accepted => "مقبول",
                Participant => "مشارك",
                Served => "تلقى الخدمة",
                Cancelled => "ملغي",
                Waiting => "قائمة انتظار",
                _ => "غير محدد"
            };
        }
    }
}
