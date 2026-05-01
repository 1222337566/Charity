using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.OldRecords
{
    public class CreateBeneficiaryOldRecordVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "تاريخ السجل مطلوب")]
        [Display(Name = "تاريخ السجل")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "العنوان مطلوب")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "التفاصيل")]
        public string? Details { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
