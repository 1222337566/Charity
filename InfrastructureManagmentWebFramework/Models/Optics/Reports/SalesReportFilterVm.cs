using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SalesReportFilterVm
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? WarehouseId { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}
