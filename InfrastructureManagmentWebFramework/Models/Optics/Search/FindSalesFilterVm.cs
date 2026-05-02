using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Search
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FindSalesFilterVm
    {
        public string? Q { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? WarehouseId { get; set; }

        public bool OnlyWithPrescription { get; set; }
        public bool OnlyWithWorkOrder { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}
