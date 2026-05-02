using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class IncomeStatementResult
    {
        public Guid? FiscalPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<IncomeStatementRow> RevenueRows { get; set; } = new();
        public List<IncomeStatementRow> ExpenseRows { get; set; } = new();

        public decimal TotalRevenue => RevenueRows.Sum(x => x.Amount);
        public decimal TotalExpense => ExpenseRows.Sum(x => x.Amount);
        public decimal NetSurplusOrDeficit => TotalRevenue - TotalExpense;
    }
}
