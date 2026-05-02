using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public class CustomerClient
    {
        public Guid Id { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }

        public CustomerGender Gender { get; set; }
        public int? Age { get; set; }

        public string? Tel { get; set; }
        public string? MobileNo { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
