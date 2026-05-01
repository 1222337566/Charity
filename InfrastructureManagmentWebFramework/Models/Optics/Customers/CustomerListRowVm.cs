using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Customers
{
    public class CustomerListRowVm
    {
        public Guid Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public string? Tel { get; set; }

        public DateTime? LastPrescriptionDateUtc { get; set; }
        public int SoldItemsCount { get; set; }
        public decimal AccountBalance { get; set; }
        public int OpenWorkOrdersCount { get; set; }

        public bool IsActive { get; set; }
    }
}
