using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class SupplierDetailsVm
    {
        public Guid Id { get; set; }
        public string SupplierNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Tel { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public int PurchasesCount { get; set; }
        public List<SupplierPurchaseItemVm> Purchases { get; set; } = new();
    }
}
