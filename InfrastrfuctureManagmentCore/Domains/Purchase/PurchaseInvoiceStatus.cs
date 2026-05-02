using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Purchase
{
    public enum PurchaseInvoiceStatus
    {
        Draft = 1,
        Posted = 2,
        Cancelled = 3,
        Approved= 4
    }
}
