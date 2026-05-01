using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Item
{
    public class ItemGroup
    {
        public Guid Id { get; set; }

        public string GroupCode { get; set; } = string.Empty;
        public string GroupNameAr { get; set; } = string.Empty;
        public string? GroupNameEn { get; set; }

        public Guid? ParentGroupId { get; set; }
        public ItemGroup? ParentGroup { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<ItemGroup> Children { get; set; } = new List<ItemGroup>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
