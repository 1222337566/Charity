using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Item
{
    public class Unit
    {
        public Guid Id { get; set; }

        public string UnitCode { get; set; } = string.Empty;
        public string UnitNameAr { get; set; } = string.Empty;
        public string? UnitNameEn { get; set; }
        public string? Symbol { get; set; }   // مثل pcs / kg / m

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
