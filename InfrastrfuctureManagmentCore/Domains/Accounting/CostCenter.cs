using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class CostCenter
    {
        public Guid Id { get; set; }
        public string CostCenterCode { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? CostCenterNameEn { get; set; }

        public Guid? ParentCostCenterId { get; set; }
        public CostCenter? ParentCostCenter { get; set; }
        public ICollection<CostCenter> Children { get; set; } = new List<CostCenter>();

        public int Level { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsProjectRelated { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        
    }
}
