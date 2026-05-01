using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Dashboard
{
    public class OpticsDashboardVm
    {
        public int CustomersCount { get; set; }
        public int ActiveCustomersCount { get; set; }
        public int CustomersWithBalanceCount { get; set; }

        public int SalesTodayCount { get; set; }
        public decimal SalesTodayTotal { get; set; }

        public int PurchasesTodayCount { get; set; }
        public decimal PurchasesTodayTotal { get; set; }

        public int ReceiptsTodayCount { get; set; }
        public decimal ReceiptsTodayTotal { get; set; }

        public int ExpensesTodayCount { get; set; }
        public decimal ExpensesTodayTotal { get; set; }

        public int WorkOrdersNewCount { get; set; }
        public int WorkOrdersInLabCount { get; set; }
        public int WorkOrdersReadyCount { get; set; }
        public int WorkOrdersOverdueCount { get; set; }

        public List<OpticsDashboardOrderVm> ReadyOrders { get; set; } = new();
        public List<OpticsDashboardInvoiceVm> LatestSales { get; set; } = new();
        public List<OpticsDashboardReceiptVm> LatestReceipts { get; set; } = new();
    }

    public class OpticsDashboardOrderVm
    {
        public Guid Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime? ExpectedDeliveryDateUtc { get; set; }
        public bool IsUrgent { get; set; }
    }

    public class OpticsDashboardInvoiceVm
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal NetAmount { get; set; }
        public DateTime InvoiceDateUtc { get; set; }
    }

    public class OpticsDashboardReceiptVm
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ReceiptDateUtc { get; set; }
    }
}
