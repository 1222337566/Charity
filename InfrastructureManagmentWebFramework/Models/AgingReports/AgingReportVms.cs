using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.AgingReports
{
    public class CustomerAgingReportPageVm
    {
        [Display(Name = "حتى تاريخ")]
        public DateTime AsOfDate { get; set; } = DateTime.Today;

        [Display(Name = "العميل")]
        public Guid? CustomerId { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
        public List<AgingReportRowVm> Rows { get; set; } = new();
        public AgingTotalsVm Totals => AgingTotalsVm.FromRows(Rows);
    }

    public class SupplierAgingReportPageVm
    {
        [Display(Name = "حتى تاريخ")]
        public DateTime AsOfDate { get; set; } = DateTime.Today;

        [Display(Name = "المورد")]
        public Guid? SupplierId { get; set; }

        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<AgingReportRowVm> Rows { get; set; } = new();
        public AgingTotalsVm Totals => AgingTotalsVm.FromRows(Rows);
    }

    public class AgingReportRowVm
    {
        public Guid PartyId { get; set; }
        public string PartyNumber { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;

        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaidOrReceived { get; set; }

        public decimal Current0To30 { get; set; }
        public decimal Days31To60 { get; set; }
        public decimal Days61To90 { get; set; }
        public decimal Over90 { get; set; }

        public decimal TotalOutstanding => Current0To30 + Days31To60 + Days61To90 + Over90;

        public List<AgingInvoiceLineVm> OpenInvoices { get; set; } = new();
    }

    public class AgingInvoiceLineVm
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; }
        public int AgeDays { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal AllocatedPayment { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Bucket { get; set; } = string.Empty;
    }

    public class AgingTotalsVm
    {
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaidOrReceived { get; set; }
        public decimal Current0To30 { get; set; }
        public decimal Days31To60 { get; set; }
        public decimal Days61To90 { get; set; }
        public decimal Over90 { get; set; }
        public decimal TotalOutstanding { get; set; }

        public static AgingTotalsVm FromRows(IEnumerable<AgingReportRowVm> rows)
        {
            var list = rows.ToList();
            return new AgingTotalsVm
            {
                TotalInvoiced = list.Sum(x => x.TotalInvoiced),
                TotalPaidOrReceived = list.Sum(x => x.TotalPaidOrReceived),
                Current0To30 = list.Sum(x => x.Current0To30),
                Days31To60 = list.Sum(x => x.Days31To60),
                Days61To90 = list.Sum(x => x.Days61To90),
                Over90 = list.Sum(x => x.Over90),
                TotalOutstanding = list.Sum(x => x.TotalOutstanding)
            };
        }
    }
}
