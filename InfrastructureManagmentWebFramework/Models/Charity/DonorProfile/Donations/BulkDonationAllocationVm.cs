using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class BulkDonationAllocationVm
    {
        public Guid DonationId { get; set; }

        [Required]
        [Display(Name = "تاريخ التوزيع")]
        [DataType(DataType.Date)]
        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات عامة")]
        public string? Notes { get; set; }

        public decimal DonationAmount { get; set; }
        public decimal DonationRemainingAmount { get; set; }
        public string? DonationType { get; set; }
        public string? AidTypeName { get; set; }
        public string? TargetingScopeCode { get; set; }
        public string? TargetingScopeName { get; set; }
        public string? GeneralPurposeName { get; set; }
        public string? AllocationPolicyMessage { get; set; }
        public bool CanAllocateToRequests { get; set; } = true;

        public List<BulkDonationAllocationRowVm> Rows { get; set; } = new();
    }

    public class BulkDonationAllocationRowVm
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

        [Display(Name = "المبلغ الموزع الآن")]
        [Range(0, 999999999, ErrorMessage = "المبلغ يجب أن يكون رقمًا موجبًا أو صفرًا")]
        public decimal? AllocateAmount { get; set; }
    }
}
