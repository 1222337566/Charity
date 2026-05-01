using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Company
{
    public class CompanyProfile
    {
        public Guid Id { get; set; }

        public string CompanyNameAr { get; set; } = string.Empty;
        public string? CompanyNameEn { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }

        public string? ReceiptHeaderText { get; set; }
        public string? ReceiptFooterText { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
