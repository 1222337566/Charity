using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidDisbursements
{
    public class CreateBeneficiaryAidDisbursementVm
    {
        [Required]
        [Display(Name = "المستفيد")]
        public Guid BeneficiaryId { get; set; }

        public List<SelectListItem> Beneficiaries { get; set; } = new();

        [Display(Name = "طلب المساعدة المرتبط")]
        public Guid? AidRequestId { get; set; }

        [Required(ErrorMessage = "نوع المساعدة مطلوب")]
        [Display(Name = "نوع المساعدة")]
        public Guid AidTypeId { get; set; }

        [Required(ErrorMessage = "تاريخ الصرف مطلوب")]
        [Display(Name = "تاريخ الصرف")]
        [DataType(DataType.Date)]
        public DateTime DisbursementDate { get; set; } = DateTime.Today;

        [Display(Name = "المبلغ المصروف")]
        public decimal? Amount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "الحساب المالي")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "المشروع المرتبط")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "التبرع المخصص للطلب")]
        public Guid? DonationId { get; set; }

        [Display(Name = "اتفاقية التمويل")]
        public Guid? GrantAgreementId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public decimal RequestedAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public decimal AlreadyDisbursedAmount { get; set; }
        public decimal RemainingToFundAmount { get; set; }
        public decimal RemainingToDisburseAmount { get; set; }
        public decimal RemainingOnRequestAmount { get; set; }
        public decimal SelectedDonationRemainingAmount { get; set; }
        public string FundingStatusCode { get; set; } = string.Empty;
        public string FundingStatusName { get; set; } = string.Empty;
        public string DisbursementStatusCode { get; set; } = string.Empty;
        public string DisbursementStatusName { get; set; } = string.Empty;

        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<SelectListItem> AidRequests { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Donations { get; set; } = new();
        public List<SelectListItem> GrantAgreements { get; set; } = new();
    }
}
