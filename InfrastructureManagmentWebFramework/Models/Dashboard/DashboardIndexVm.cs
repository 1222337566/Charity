using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Dashboard
{
    public class DashboardIndexVm
    {
        public int ItemsCount { get; set; }
        public int WarehousesCount { get; set; }
        public int SalesTodayCount { get; set; }
        public int PurchasesTodayCount { get; set; }
        public int HeldInvoicesCount { get; set; }
        public int StockBalancesCount { get; set; }
        public int LowStockCount { get; set; }
        public int ActivePaymentMethodsCount { get; set; }

        public List<DashboardInvoiceVm> LatestSales { get; set; } = new List<DashboardInvoiceVm>();
        public List<DashboardInvoiceVm> LatestPurchases { get; set; } = new List<DashboardInvoiceVm>();
    }

    public class DashboardInvoiceVm
    {
        public string Number { get; set; } = string.Empty;
        public DateTime DateUtc { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal NetAmount { get; set; }
    }
}
