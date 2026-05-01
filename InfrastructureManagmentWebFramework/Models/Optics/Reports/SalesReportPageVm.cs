using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    public class SalesReportPageVm
    {
        public SalesReportFilterVm Filter { get; set; } = new();
        public SalesReportVm? Report { get; set; }
    }
}
