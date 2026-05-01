using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Payments
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }

        public string MethodCode { get; set; } = string.Empty;
        public string MethodNameAr { get; set; } = string.Empty;
        public string? MethodNameEn { get; set; }

        public bool IsCash { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
