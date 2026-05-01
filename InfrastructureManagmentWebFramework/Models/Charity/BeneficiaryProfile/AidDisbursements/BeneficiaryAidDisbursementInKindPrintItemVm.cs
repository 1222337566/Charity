using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidDisbursements
{
    public class BeneficiaryAidDisbursementInKindPrintItemVm
    {
        public Guid? DonationAllocationId { get; set; }
        public Guid? DonationInKindItemId { get; set; }
        public Guid? StoreIssueLineId { get; set; }

        public string? ItemCode { get; set; }
        public string ItemName { get; set; } = "-";
        public string? WarehouseName { get; set; }
        public string? DonationNumber { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public decimal Quantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }

        public string? Notes { get; set; }
    }
}
