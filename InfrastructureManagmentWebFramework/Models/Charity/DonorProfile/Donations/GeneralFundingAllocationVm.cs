using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class GeneralFundingAllocationVm
    {
        [Display(Name = "نطاق الرصيد")]
        public string SourceScopeCode { get; set; } = "GeneralFund";

        [Display(Name = "الحساب المالي المصدر")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "نوع المساعدة")]
        public Guid? AidTypeId { get; set; }

        [Display(Name = "الغرض / الباب العام")]
        public string? PurposeName { get; set; }

        [Required]
        [Display(Name = "تاريخ التخصيص")]
        [DataType(DataType.Date)]
        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات عامة")]
        public string? Notes { get; set; }

        public decimal AvailablePoolAmount { get; set; }
        public decimal SelectedTotalAmount { get; set; }
        public string? FinancialAccountName { get; set; }
        public string? AidTypeName { get; set; }

        public List<SelectListItem> SourceScopes { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<GeneralFundingSourceDonationVm> SourceDonations { get; set; } = new();
        public List<GeneralFundingAllocationRowVm> Rows { get; set; } = new();
    }

    public class GeneralFundingSourceDonationVm
    {
        public Guid DonationId { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string? DonorName { get; set; }
        public decimal DonationAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string? PurposeName { get; set; }
        public string? AidTypeName { get; set; }
    }

    public class GeneralFundingAllocationRowVm
    {
        public Guid AidRequestId { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string BeneficiaryName { get; set; } = string.Empty;
        public string RequestNumberOrDate { get; set; } = string.Empty;
        public string AidTypeName { get; set; } = string.Empty;
        public string? UrgencyLevel { get; set; }
        public bool HasPreviousDisbursement { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal RemainingNeedAmount { get; set; }

        [Display(Name = "المبلغ المخصص الآن")]
        [Range(0, 999999999, ErrorMessage = "المبلغ يجب أن يكون رقمًا موجبًا أو صفرًا")]
        public decimal? AllocateAmount { get; set; }
    }
}
