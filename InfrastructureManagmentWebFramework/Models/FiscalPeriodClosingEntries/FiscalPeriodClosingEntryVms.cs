using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.FiscalPeriodClosingEntries
{
    public class FiscalPeriodClosingEntriesIndexVm
    {
        public List<FiscalPeriodClosingEntryPeriodRowVm> Periods { get; set; } = new();
    }

    public class FiscalPeriodClosingEntryPeriodRowVm
    {
        public Guid FiscalPeriodId { get; set; }

        public string PeriodName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsClosed { get; set; }

        public bool HasClosingEntry { get; set; }

        public string? ClosingEntryNumber { get; set; }

        public Guid? ClosingEntryId { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal NetSurplusOrDeficit => TotalRevenue - TotalExpense;
    }

    public class FiscalPeriodClosingEntryPrepareVm
    {
        public Guid FiscalPeriodId { get; set; }

        public string PeriodName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool HasClosingEntry { get; set; }

        public Guid? ExistingClosingEntryId { get; set; }

        public string? ExistingClosingEntryNumber { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal NetSurplusOrDeficit => TotalRevenue - TotalExpense;

        [Required(ErrorMessage = "برجاء اختيار حساب الفائض/العجز المرحّل")]
        [Display(Name = "حساب الفائض/العجز المرحّل")]
        public Guid RetainedSurplusAccountId { get; set; }

        [Required]
        [Display(Name = "تاريخ قيد الإقفال")]
        public DateTime ClosingEntryDateUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "ملاحظات")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public List<SelectListItem> EquityAccounts { get; set; } = new();

        public List<FiscalPeriodClosingEntryAccountRowVm> RevenueRows { get; set; } = new();

        public List<FiscalPeriodClosingEntryAccountRowVm> ExpenseRows { get; set; } = new();

        public decimal TotalDebitPreview => RevenueRows.Sum(x => Math.Max(0, x.NetCreditBalance)) + Math.Max(0, -NetSurplusOrDeficit);

        public decimal TotalCreditPreview => ExpenseRows.Sum(x => Math.Max(0, x.NetDebitBalance)) + Math.Max(0, NetSurplusOrDeficit);
    }

    public class FiscalPeriodClosingEntryAccountRowVm
    {
        public Guid AccountId { get; set; }

        public string AccountCode { get; set; } = string.Empty;

        public string AccountNameAr { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public decimal NetDebitBalance => DebitAmount - CreditAmount;

        public decimal NetCreditBalance => CreditAmount - DebitAmount;

        public decimal ClosingAmount => Math.Abs(DebitAmount - CreditAmount);
    }

    public class CreateFiscalPeriodClosingEntryVm
    {
        [Required]
        public Guid FiscalPeriodId { get; set; }

        [Required]
        public Guid RetainedSurplusAccountId { get; set; }

        [Required]
        public DateTime ClosingEntryDateUtc { get; set; }

        public string? Notes { get; set; }
    }
}
