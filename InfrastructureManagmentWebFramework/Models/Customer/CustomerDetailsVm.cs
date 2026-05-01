using InfrastructureManagmentWebFramework.Models.Optics;
using InfrastructureManagmentWebFramework.Models.Optics.OldRecords;
using InfrastructureManagmentWebFramework.Models.SoldItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Customer
{
    public class CustomerDetailsVm
    {
        public Guid Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string GenderText { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Tel { get; set; }
        public string? MobileNo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public List<CustomerOldRecordItemVm> OldRecords { get; set; } = new();
        public List<PrescriptionListItemVm> Prescriptions { get; set; } = new();
        public List<CustomerSoldItemVm> SoldItems { get; set; } = new();
        public List<CustomerAccountEntryVm> AccountEntries { get; set; } = new();
        public int PrescriptionsCount { get; set; }
        public int SoldItemsCount { get; set; }
        public decimal AccountBalance { get; set; }
        //public decimal AccountEntries { get; set; }
        public int OldRecordsCount
        {
            get; set;
        }
    }
    }

