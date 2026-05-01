using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    public class SalesReportVm
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public int InvoiceCount { get; set; }
        public decimal NetAmountTotal { get; set; }
        public decimal TotalPaid { get; set; }
        public int PrescriptionInvoicesCount { get; set; }
        public int WorkOrdersCount { get; set; }

        public List<SalesReportRowVm> Rows { get; set; } = new();
    }
}
