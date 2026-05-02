using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Warehouses
{
    public class Warehouse
    {
        public Guid Id { get; set; }

        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseNameAr { get; set; } = string.Empty;
        public string? WarehouseNameEn { get; set; }

        public string? Address { get; set; }
        public string? Notes { get; set; }

        public bool IsMain { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
