using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Beneficiaries
{
    public class VerifyActivityBeneficiaryVm
    {
        public Guid ActivityBeneficiaryId { get; set; }

        [Display(Name = "حالة الإثبات")]
        [Required]
        public string VerificationStatus { get; set; } = "Unverified";

        [Display(Name = "ملاحظات الإثبات")]
        public string? VerificationNotes { get; set; }

        [Display(Name = "تاريخ الإثبات")]
        [DataType(DataType.Date)]
        public DateTime VerificationDate { get; set; } = DateTime.Today;

        [Display(Name = "نوع المرفق")]
        public string? AttachmentType { get; set; }

        [Display(Name = "مرفقات الإثبات")]
        public List<IFormFile>? NewAttachments { get; set; }

        public List<string> VerificationStatuses { get; set; } = new()
        {
            "Unverified", "Verified", "Rejected", "NeedsReview"
        };

        public static string GetStatusAr(string status) => status switch
        {
            "Unverified"  => "غير مثبت",
            "Verified"    => "مثبت",
            "Rejected"    => "مرفوض",
            "NeedsReview" => "يحتاج مراجعة",
            _ => status
        };

        public static string GetAttachmentTypeAr(string type) => type switch
        {
            "AttendancePhoto"    => "صورة حضور",
            "Signature"          => "توقيع",
            "ParticipationForm"  => "استمارة مشاركة",
            "IdPhoto"            => "صورة هوية",
            "ActivityReport"     => "تقرير نشاط",
            _ => "أخرى"
        };
    }
}
