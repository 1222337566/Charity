using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced
{
    public class StockReturnVoucher
    {
        public Guid Id { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; } = DateTime.UtcNow.Date;
        public Guid WarehouseId { get; set; }
        public string ReturnType { get; set; } = "Project";
        public Guid? ProjectId { get; set; }
        public Guid? BeneficiaryId { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public List<StockReturnVoucherLine> Lines { get; set; } = new();
    }
}
