using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.PosHolds
{
    public class PosHold
    {
        public Guid Id { get; set; }

        public string HoldNumber { get; set; } = string.Empty;
        public DateTime HoldDateUtc { get; set; } = DateTime.UtcNow;

        public string CustomerName { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public PosHoldStatus Status { get; set; } = PosHoldStatus.Held;

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<PosHoldLine> Lines { get; set; } = new List<PosHoldLine>();
    }
}
