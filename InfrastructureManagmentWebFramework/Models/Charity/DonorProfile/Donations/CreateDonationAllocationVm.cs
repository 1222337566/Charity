using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class CreateDonationAllocationVm : IValidatableObject
    {
        public Guid DonationId { get; set; }

        [Required]
        [Display(Name = "تاريخ التخصيص")]
        [DataType(DataType.Date)]
        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "طلب المساعدة مطلوب")]
        [Display(Name = "طلب المساعدة")]
        public Guid? AidRequestId { get; set; }

        [Display(Name = "المستفيد")]
        public Guid? BeneficiaryId { get; set; }

        [Display(Name = "بند طلب المساعدة")]
        public Guid? AidRequestLineId { get; set; }

        [Display(Name = "الصنف العيني")]
        public Guid? DonationInKindItemId { get; set; }

        [Display(Name = "الكمية المخصصة")]
        [Range(0, 999999999)]
        public decimal? AllocatedQuantity { get; set; }

        [Display(Name = "المبلغ المخصص")]
        [Range(0, 999999999)]
        public decimal? Amount { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> AidRequests { get; set; } = new();
        public List<SelectListItem> AidRequestLines { get; set; } = new();
        public List<SelectListItem> DonationInKindItems { get; set; } = new();

        public decimal DonationRemainingAmount { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal AlreadyAllocatedAmount { get; set; }
        public decimal RemainingNeedAmount { get; set; }
        public decimal AlreadyDisbursedAmount { get; set; }

        public string? ApprovalStatusCode { get; set; }
        public string? ApprovalStatusName { get; set; }

        public string? TargetingScopeCode { get; set; }
        public string? TargetingScopeName { get; set; }
        public string? GeneralPurposeName { get; set; }
        public string? AllocationPolicyMessage { get; set; }
        public bool CanAllocateToRequests { get; set; } = true;
        public bool SelectedRequestHasLines { get; set; }
        public decimal SelectedLineRequestedQuantity { get; set; }
        public decimal SelectedLineApprovedQuantity { get; set; }
        public decimal SelectedLineEstimatedTotal { get; set; }
        public decimal SelectedLineAllocatedAmount { get; set; }
        public decimal SelectedLineAllocatedQuantity { get; set; }
        public decimal SelectedLineRemainingAmount { get; set; }
        public decimal SelectedLineRemainingQuantity { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasAmount = Amount.HasValue && Amount.Value > 0;
            var hasItem = DonationInKindItemId.HasValue;
            var hasQty = AllocatedQuantity.HasValue && AllocatedQuantity.Value > 0;

            if (!AidRequestId.HasValue)
            {
                yield return new ValidationResult("يجب اختيار طلب مساعدة معتمد قبل التخصيص.", new[] { nameof(AidRequestId) });
            }

            if (!hasAmount && !(hasItem && hasQty))
            {
                yield return new ValidationResult("أدخل مبلغًا مخصصًا أو اختر صنفًا عينيًا مع كمية.", new[] { nameof(Amount), nameof(DonationInKindItemId), nameof(AllocatedQuantity) });
            }

            if (hasItem && !hasQty)
            {
                yield return new ValidationResult("عند اختيار صنف عيني يجب إدخال الكمية المخصصة.", new[] { nameof(AllocatedQuantity) });
            }
        }
    }
}
