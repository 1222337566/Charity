using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    public class CustomerReportItemVm
    {
        public DateTime DateUtc { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? RefNo { get; set; }
        public string? Details { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public string? StatusText { get; set; }
    }
}
