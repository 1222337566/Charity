using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.InventoryValuation
{
    public class InventoryValuationPageVm
    {
        public Guid? WarehouseId { get; set; }
        public DateTime AsOfDateUtc { get; set; } = DateTime.UtcNow;

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<InventoryValuationRowVm> Rows { get; set; } = new();

        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}
