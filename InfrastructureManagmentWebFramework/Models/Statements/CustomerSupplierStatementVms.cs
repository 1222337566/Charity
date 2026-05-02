using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.Statements
{
    public class CustomerStatementPageVm
    {
        [Display(Name = "العميل")]
        public Guid? CustomerId { get; set; }

        [Display(Name = "من تاريخ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "إلى تاريخ")]
        public DateTime? ToDate { get; set; }

        public string? CustomerNumber { get; set; }
        public string? CustomerName { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
        public List<StatementLineVm> Lines { get; set; } = new();

        public decimal TotalDebit => Lines.Sum(x => x.Debit);
        public decimal TotalCredit => Lines.Sum(x => x.Credit);
        public decimal ClosingBalance => Lines.LastOrDefault()?.RunningBalance ?? 0;
    }

    public class SupplierStatementPageVm
    {
        [Display(Name = "المورد")]
        public Guid? SupplierId { get; set; }

        [Display(Name = "من تاريخ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "إلى تاريخ")]
        public DateTime? ToDate { get; set; }

        public string? SupplierNumber { get; set; }
        public string? SupplierName { get; set; }

        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<StatementLineVm> Lines { get; set; } = new();

        public decimal TotalDebit => Lines.Sum(x => x.Debit);
        public decimal TotalCredit => Lines.Sum(x => x.Credit);
        public decimal ClosingBalance => Lines.LastOrDefault()?.RunningBalance ?? 0;
    }

    public class StatementLineVm
    {
        public DateTime DateUtc { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentTypeAr { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RunningBalance { get; set; }
        public string? Notes { get; set; }
        public Guid? SourceId { get; set; }
    }
}
