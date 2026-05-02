using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    public class CustomerReportPageVm
    {
        public CustomerReportFilterVm Filter { get; set; } = new();
        public CustomerReportVm? Report { get; set; }
    }
}
