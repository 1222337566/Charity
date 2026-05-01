using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CustomerReportFilterVm
    {
        public Guid? CustomerId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
    }
}
