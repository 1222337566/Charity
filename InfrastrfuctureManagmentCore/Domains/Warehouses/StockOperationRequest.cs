using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Warehouses
{
    public class StockOperationRequest
    {
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        public DateTime? TransactionDateUtc { get; set; }

        public string? ReferenceType { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid? ReferenceId { get; set; }

        public string? Notes { get; set; }
    }
}
