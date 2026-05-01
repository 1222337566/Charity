using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Reports
{
    public class CustomerReportVm
    {
        public Guid CustomerId { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string GenderText { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Tel { get; set; }
        public string? MobileNo { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public int PrescriptionsCount { get; set; }
        public int SoldItemsCount { get; set; }
        public int OldRecordsCount { get; set; }
        public int WorkOrdersCount { get; set; }
        public decimal AccountBalance { get; set; }

        public List<CustomerReportItemVm> Prescriptions { get; set; } = new();
        public List<CustomerReportItemVm> Sales { get; set; } = new();
        public List<CustomerReportItemVm> AccountEntries { get; set; } = new();
        public List<CustomerReportItemVm> OldRecords { get; set; } = new();
        public List<CustomerReportItemVm> WorkOrders { get; set; } = new();
    }
}
