using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public class CustomerOldRecord
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerClient? Customer { get; set; }

        public DateTime RecordDateUtc { get; set; } = DateTime.UtcNow;

        public string Title { get; set; } = string.Empty;
        public string? Details { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
