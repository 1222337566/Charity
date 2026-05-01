using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class AccountDetailsVm
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string? AccountNameEn { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsPosting { get; set; }
        public bool IsActive { get; set; }
        public bool RequiresProject { get; set; }
        public bool RequiresCostCenter { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string? ParentAccountDisplay { get; set; }

        public Guid? FiscalPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SelectListItem> FiscalPeriods { get; set; } = new();

        public AccountStatementResult? Statement { get; set; }
        public bool HasStatement => Statement != null;
        public int MovementCount => Statement?.Rows.Count ?? 0;
        public string ClosingBalanceNature => (Statement?.ClosingBalance ?? 0m) >= 0 ? "مدين" : "دائن";
    }
}
