using System;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class IncomeStatementRow
    {
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Amount { get; set; }
    }
}
