using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Documents
{
    public class CreateBeneficiaryDocumentVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "نوع المستند مطلوب")]
        [Display(Name = "نوع المستند")]
        public string DocumentType { get; set; } = string.Empty;

        [Display(Name = "رقم المستند")]
        public string? DocumentNumber { get; set; }

        [Display(Name = "تاريخ الإصدار")]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "تاريخ الانتهاء")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "مسار الملف")]
        public string? FilePath { get; set; }

        [Display(Name = "موثق")]
        public bool IsVerified { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
